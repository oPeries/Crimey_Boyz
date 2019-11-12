/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System;

//Used to identify sever / client IP addresses over a LAN network (subnet traversal not tested)
//Two modes / instances: Transmitter & Listener
//When Transmitter:    sends out packets Listens for a response and saves the Listener's IP address
//                          Note: stops transmitting / listening for a response once a response is received
//
//When Listener:       listens for broadcast messages and responds via unicast to let the Broadcaster know the IP address of this Listener instance
//
//There are 4 methods to choose from when doing LAN discovery
//LOCALIP - The LAN discovery does not begin and sets the response Address to 127.0.0.1 automatically
//DIRECTIP - Similar to LOCALIP but sets the response IP to the directIP variable
//BROADCAST - Broadcast packets are sent to all IPv4 subnets this device is attached to.
//MULTICAST - Initiates communication of the multicast group specified by the multicastIP variable
public class LANdiscovery : MonoBehaviour {

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Defines ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private const int THREAD_SLEEP_TIME = 200;//(ms) time the listening thread sleeps for

    public enum DiscoveryMethod {
        LOCALIP,
        DIRECTIP,
        BROADCAST,
        MULTICAST
    }


    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public static LANdiscovery Singleton { get; private set; } //Singleton instance

    public float broadcastRate = 2f; //#times per second the client will send a broadcast packet
    public int discoveryPort = 8002; //port to send and receive UDP packets
    public string directIP = "127.0.0.1"; //specific IP for the server (if using direct method)
    public string multicastIP = "224.0.2.222"; //multicast IP address to use (if using multicast method)
    public DiscoveryMethod discoveryMethod;

    private List<IPAddress> localAddresses; //IPv4 addresses for this device
    private List<IPAddress> localSubnets; //IPv4 subnets this device is attached to
    private List<IPAddress> localBroadcasts; //IPv4 broadcast addresses for this device's subnets

    private static UdpClient netClient = null; //socket to send and receive data on
    private static Thread broadcastThread = null; //thread for sending UDP packets
    private static Thread listenThread = null; //thread for receiving UDP packets

    volatile IPAddress responseAddress = null; //IPv4 address of the Listener device (null until response received, will be the IP from the first response)


    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Behavioural ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    //Initialise
    private void Awake() {

        localAddresses = new List<IPAddress>();
        localSubnets = new List<IPAddress>();
        localBroadcasts = new List<IPAddress>();

        FindLocalIPConfigs();

        //Debug.Log("Found " + localAddresses.Count.ToString() + " relevant addresses for this device");
        //for (int i = 0; i < localAddresses.Count; i++) {
        //    Debug.Log(localAddresses[i].ToString() + " : " + localSubnets[i].ToString() + " : " + localBroadcasts[i].ToString());
        //}
    }

    //Sets the singleton for this class. Ensures only one copy of this script is instantiated.
    private void OnEnable() {

        if (Singleton != null && this != Singleton) {
            Destroy(this.gameObject);
        } else {
            Singleton = this;
        }
    }

    private void OnDestroy() {
        //Debug.Log("On Destroy");
        Stop();
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public List<IPAddress> GetLocalAddresses() {
        FindLocalIPConfigs();
        return new List<IPAddress>(localAddresses);
    }

    /// <summary>
    /// Should return null if response has not been found
    /// </summary>
    /// <returns>
    /// The IPAddress containing the response address
    /// </returns>
    public IPAddress GetResponseIP() {
        return responseAddress;
    }

    /// <summary>
    /// Try to start as a Broadcaster.
    /// Returns true if successful, false otherwise
    /// </summary>
    /// <returns>
    /// Returns true if broadcasting is successful, false if otherwise
    /// </returns>
    [Obsolete("Use StartTransmissing instead")]
    public bool StartBroadcast() {
        return StartTransmitting();
    }

    /// <summary>
    /// Try to start as a Broadcaster.
    /// Returns true if successful, false otherwise
    /// </summary>
    /// <returns>
    /// Returns true if broadcasting is successful, false if otherwise
    /// </returns>
    public bool StartTransmitting() {

        responseAddress = null;

        //If using local host, don't listen or broadcast, just set ip immediately
        if (discoveryMethod == DiscoveryMethod.DIRECTIP) {
            responseAddress = IPAddress.Parse(directIP);
            return true;
        } else if (discoveryMethod == DiscoveryMethod.LOCALIP) {
            responseAddress = IPAddress.Parse("127.0.0.1");
            return true;
        }

        if(!startSocket()) {
            Debug.LogError("Failed to start socket");
            return false;
        }

        if (broadcastThread != null || listenThread != null) {
            Debug.LogError("Warning: Tried to start broadcasting without stopping current broadcast/listen");
            return false;
        }

        //start the listening thread (as a Broadcaster)
        listenThread = new Thread(() => listen(false));
        listenThread.IsBackground = true;
        listenThread.Start();

        //start broadcasting thread
        broadcastThread = new Thread(new ThreadStart(broadcast));
        broadcastThread.IsBackground = true;
        broadcastThread.Start();

        //check both threads are alive
        if (broadcastThread != null && listenThread != null && listenThread.IsAlive && broadcastThread.IsAlive) {
            //Debug.Log("Searching for a server");
            return true;
        } else {
            //Failed to start both threads, stop/end whatever is left over
            Debug.LogError("Failed to start LAN Discovery Broadcaster threads");
            Stop();
            return false;
        }
    }

    /// <summary>
    /// Try to start as a Listener 
    /// </summary>
    /// <returns>
    /// Returns true if successful, false otherwise
    /// </returns>
    public bool StartListening() {

        responseAddress = null;

        //If using local host, don't listen or broadcast, just set ip immediately
        if (discoveryMethod == DiscoveryMethod.LOCALIP || discoveryMethod == DiscoveryMethod.DIRECTIP) {
            return true;
        }

        if (!startSocket()) {
            Debug.LogError("Failed to start socket");
            return false;
        }

        if (broadcastThread != null || listenThread != null) {
            Debug.LogError("Warning: Tried to start listening without stopping current broadcast/listen");
            return false;
        }

        //start the listening thread (as a Listener)
        listenThread = new Thread(() => listen(true));
        listenThread.IsBackground = true;
        listenThread.Start();

        //check thread is alive
        if (listenThread != null && listenThread.IsAlive) {
            //Debug.Log("Listening for a client");
            return true;
        } else {
            Debug.LogError("Failed to start LAN Discovery Listener threads");
            Stop();
            return false;
        }
    }

    /// <summary>
    /// Cleanup - Stop broadcasting / listening (if enabled)
    /// </summary>
    public void Stop() {
        //stop listening thread
        if (listenThread != null) {
            listenThread.Interrupt();
            listenThread.Join();
            if (!listenThread.IsAlive) {
                listenThread = null;
            }
        }

        //stop broadcasting thread
        if (broadcastThread != null) {
            broadcastThread.Interrupt();
            broadcastThread.Join();
            if (!broadcastThread.IsAlive) {
                broadcastThread = null;
            }
        }

        if (listenThread != null || broadcastThread != null) {
            Debug.LogError("Failed to stop discovery");
        }

        if (netClient != null) {
            netClient.Close();
            netClient = null;
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Helper Functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    /// <summary>
    /// Initialises the socket
    /// </summary>
    private bool startSocket() {
        if (discoveryMethod != DiscoveryMethod.LOCALIP && discoveryMethod != DiscoveryMethod.DIRECTIP) {
            if (netClient == null) {
                try {
                    netClient = new UdpClient();

                    if (discoveryMethod == DiscoveryMethod.BROADCAST) {
                        netClient.Client.Bind(new IPEndPoint(IPAddress.Any, discoveryPort));
                        netClient.EnableBroadcast = true;
                    }

                    if (discoveryMethod == DiscoveryMethod.MULTICAST) {
                        netClient.ExclusiveAddressUse = false;
                        netClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                        netClient.ExclusiveAddressUse = false;

                        netClient.Client.Bind(new IPEndPoint(IPAddress.Any, discoveryPort));

                        netClient.JoinMulticastGroup(IPAddress.Parse(multicastIP));
                    }

                    return true;

                } catch (SocketException e) {
                    Debug.LogError("Failed to start Discovery socket: \n" + e.ToString());
                    return false;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Thread - listen for listens for incoming UDP messages
    /// if isResponder, will check for IPrequests and respond accordingly
    /// else, will check for IPresponses and upon receiving a response will save the response IP and exit the thread
    /// </summary>
    /// <param name="isResponder"> value for if isResponder </param>
    private void listen(bool isResponder) {

        while (true) {
            //Listen for incoming requests
            if (netClient.Available > 0) {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, discoveryPort);
                string message = Encoding.UTF8.GetString(netClient.Receive(ref endPoint));

                //Debug.Log("Received UDP Message: \"" + message + "\" from " + endPoint.Address.ToString());

                //Ignore invalid messages
                if (!message.StartsWith("DECO3801-")) {
                    continue;
                }
                message = message.Substring(9);

                if (isResponder) {
                    //If a received request is a valid client request, respond with server info
                    if (message.StartsWith("IPRequest")) {
                        byte[] response = Encoding.UTF8.GetBytes("DECO3801-IPResponse");
                        netClient.Send(response, response.Length, endPoint);
                        //Debug.Log("Server Response Sent");
                    }

                } else {
                    //If a response from a listening instance, save IP address & exit
                    if (message.StartsWith("IPResponse")) {
                        responseAddress = endPoint.Address;
                        break;
                    }
                }
            }

            try {
                Thread.Sleep(100);
            } catch (ThreadInterruptedException) {
                //Debug.Log("Stopped Listening Thread");
                break;
            }
        }
    }

    /// <summary>
    /// Thread - broadcast an IP request
    /// Sends a UDP broadcast packet requesting the IP for a server / listening instance
    /// Should send broadcast request on all connected networks
    /// Once the responseAddress variable has been set, will exit
    /// </summary>
    private void broadcast() {

        byte[] message = Encoding.UTF8.GetBytes("DECO3801-IPRequest");
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, discoveryPort);

        while (true) {

            //If a response has been received (response IP set), exit thread
            if (responseAddress != null) {
                break;
            }

            if (discoveryMethod == DiscoveryMethod.BROADCAST) {
                //Send broadcast packets through each network interface
                for (int i = 0; i < localBroadcasts.Count; i++) {
                    endPoint.Address = localBroadcasts[i];
                    netClient.Send(message, message.Length, endPoint);
                }
            }

            if (discoveryMethod == DiscoveryMethod.MULTICAST) {
                endPoint.Address = IPAddress.Parse(multicastIP);
                netClient.Send(message, message.Length, endPoint);
            }

            //Send out "broadcastRate" packets per second
            try {
                Thread.Sleep(Mathf.FloorToInt(1 / broadcastRate * 1000));
            } catch (ThreadInterruptedException) {
                //Debug.Log("Stopped Broadcasting Thread");
                break;
            }
        }
    }

    /// <summary>
    /// Get the unicast addresses, subnets and broadcast addresses for this device
    /// Adds found addresses to the localAddresses, localSubnet & localBroadcasts variables
    /// </summary>
    private void FindLocalIPConfigs() {

        localAddresses.Clear();
        localSubnets.Clear();
        localBroadcasts.Clear();

        //Find this device's unicast IP addresses (using DNS settings)
        string hostName = Dns.GetHostName();
        List<IPAddress> localDNSAddresses = new List<IPAddress>();
        foreach (IPAddress ip in Dns.GetHostAddresses(Dns.GetHostName())) {
            //Only interested in IPv4 addresses
            if (ip.AddressFamily == AddressFamily.InterNetwork) {
                localDNSAddresses.Add(ip);
            }
        }

        //All network interfaces for this device
        NetworkInterface[] allInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        //Find the subnet & broadcast IP for each IP address in localDNSAddresses
        //Add address, subnet & broadcast to list variables
        foreach (NetworkInterface adaptor in allInterfaces) {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            //If windows OS, ignore anything except Ethernet & WiFi adaptors
            if (adaptor.NetworkInterfaceType != NetworkInterfaceType.Ethernet && adaptor.NetworkInterfaceType != NetworkInterfaceType.Wireless80211) {
                continue;
            }
#endif
            //For each IP address associated with this adaptor
            foreach (UnicastIPAddressInformation ip in adaptor.GetIPProperties().UnicastAddresses) {

                //Only interested in IPv4 addresses
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork) {

                    //If this IP address matches one found using DNS settings, add to class lists
                    if (localDNSAddresses.Contains(ip.Address)) {
                        localAddresses.Add(ip.Address);
                        localSubnets.Add(ip.IPv4Mask);

                        //Calculate the broadcast address using the subnet & address
                        byte[] broadcastRaw = new byte[ip.Address.GetAddressBytes().Length];
                        for (int i = 0; i < ip.Address.GetAddressBytes().Length; i++) {
                            broadcastRaw[i] = (byte)(ip.Address.GetAddressBytes()[i] | (byte)~ip.IPv4Mask.GetAddressBytes()[i]);
                        }

                        IPAddress broadcast = new IPAddress(broadcastRaw);
                        localBroadcasts.Add(broadcast);
                    }
                }
            }
        }
    }
}
