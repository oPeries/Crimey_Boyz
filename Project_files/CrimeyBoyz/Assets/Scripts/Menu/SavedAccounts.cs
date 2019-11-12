/*
Begin license text.
Copyright 2019 Team Scruffle3 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

End license text.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SavedAccounts : MonoBehaviour {

    public class SavedUser {
        public string username;
        public string cred;

        public SavedUser(string username, string cred) {
            this.username = username;
            this.cred = cred;
        }
    }

    private static List<SavedUser> savedUsers = null;

    public static List<string> GetAllSavedUsers() {
        if(savedUsers == null) {
            LoadUsersFromCache();
        }
        List<string> result = new List<string>();
        foreach(SavedUser user in savedUsers) {
            result.Add(user.username);
        }
        return result;
    }

    public static string GetUserCred(string username) {
        if (savedUsers == null) {
            LoadUsersFromCache();
        }
        foreach (SavedUser user in savedUsers) {
            if (user.username.Equals(username)) {
                return user.cred;
            }
        }
        return null;
    }

    public static void SaveUser(string username, string cookie) {
        if (savedUsers == null) {
            LoadUsersFromCache();
        }

        bool updatedExisting = false;
        foreach (SavedUser user in savedUsers) {
            if (user.username.Equals(username)) {
                updatedExisting = true;
                user.cred = cookie;
            }
        }
        
        if(!updatedExisting) {
            savedUsers.Add(new SavedUser(username, cookie));
        }

        SaveUsersToCatche();
    }

    //Forget this saved user (if they exists)
    public static void ForgetUser(string username) {
        foreach (SavedUser user in savedUsers) {
            if(user.username.Equals(username)) {
                savedUsers.Remove(user);
                break;
            }
        }
        SaveUsersToCatche();
    }

    private static void LoadUsersFromCache() {
        int savedCount = PlayerPrefs.GetInt("Saved_Count", 0);

        savedUsers = new List<SavedUser>();

        for (int i = 0; i < savedCount; i++) {
            string encrypted = PlayerPrefs.GetString("SavedUA" + i.ToString(), null);
            if(encrypted == null) {
                continue;
            }

            string[] decrypted = Decrypt(encrypted).Split(':');
            
            if (decrypted.Length != 2) {
                Debug.LogWarning("Error when trying to load saved user: " + Decrypt(encrypted));
                continue;
            }

            SavedUser user = new SavedUser(decrypted[0], decrypted[1]);
            savedUsers.Add(user);
        }
    }

    private static void SaveUsersToCatche() {
        if(savedUsers == null) {
            return;
        }
        PlayerPrefs.SetInt("Saved_Count", savedUsers.Count);

        for (int i = 0; i < savedUsers.Count; i++) {
            string encrypted = Encrypt(savedUsers[i].username + ":" + savedUsers[i].cred);
            PlayerPrefs.SetString("SavedUA" + i.ToString(), encrypted);
        }
        LoadUsersFromCache();
    }


    //Simple encrypt and decrypt using RC4 algorithm
    //Using this to store usernames in the registry
    private static string Encrypt(string username) {

        byte[] key = Encoding.Unicode.GetBytes("16HuC#x$Zmo#");
        byte[] data = Encoding.Unicode.GetBytes(username);
        return Convert.ToBase64String(EncryptOutput(key, data));
    }

    private static string Decrypt(string encoded) {

        byte[] key = Encoding.Unicode.GetBytes("16HuC#x$Zmo#");
        byte[] data = Convert.FromBase64String(encoded);
        return Encoding.Unicode.GetString(EncryptOutput(key, data));
    }

    private static byte[] EncryptOutput(byte[] key, byte[] data) {

        int[] box = new int[256];
        int[] key2 = new int[256];

        for (int i = 0; i < 256; i++) {
            key2[i] = key[i % key.Length];
            box[i] = i;
        }

        for (int i = 0, j = 0; i < 256; i++) {

            j = (j + box[i] + key2[i]) % 256;
            int c = box[i];

            box[i] = box[j];
            box[j] = c;
        }

        int k;
        byte[] encrypted = new byte[data.Length];

        for (int j = 0, a = 0, i = 0; i < data.Length; i++) {

            a = (a + 1) % 256;
            j = (j + box[a]) & 255;

            int c = box[a];
            box[a] = box[j];
            box[j] = c;

            k = box[(box[a] + box[j]) % 256];
            encrypted[i] = (byte)(data[i] ^ k);
        }

        return encrypted;
    }
}
