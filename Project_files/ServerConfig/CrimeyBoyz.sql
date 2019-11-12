-- phpMyAdmin SQL Dump
-- version 4.5.4.1deb2ubuntu2.1
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Oct 25, 2019 at 04:23 AM
-- Server version: 5.7.27-0ubuntu0.16.04.1
-- PHP Version: 7.0.33-0ubuntu0.16.04.6

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `CrimeyBoyz`
--
CREATE DATABASE IF NOT EXISTS `CrimeyBoyz` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
USE `CrimeyBoyz`;

-- --------------------------------------------------------

--
-- Table structure for table `data`
--

CREATE TABLE `data` (
  `id` int(1) NOT NULL,
  `user_id` int(11) NOT NULL,
  `collisions` int(11) NOT NULL,
  `score` int(11) NOT NULL,
  `visits` int(15) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `interactions`
--

CREATE TABLE `interactions` (
  `interactionID` int(11) NOT NULL,
  `metricName` varchar(255) NOT NULL,
  `roundID` int(11) NOT NULL,
  `initiatingPlayerNum` int(1) NOT NULL,
  `actionTime` float NOT NULL,
  `actionXPos` float DEFAULT NULL,
  `actionYPos` float DEFAULT NULL,
  `actionSpecificData` text
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `metrics`
--

CREATE TABLE `metrics` (
  `metricName` varchar(255) NOT NULL,
  `metricDescription` text NOT NULL,
  `dataDescription` text NOT NULL,
  `behaviourType` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `metrics`
--

INSERT INTO `metrics` (`metricName`, `metricDescription`, `dataDescription`, `behaviourType`) VALUES
('ElevatorCloses', 'How many times the player hit the button to close the elevator doors.', 'Action specific data -  the number of times the button was pressed in that floor.', 'Likely selfish if not all players are in the elevator.'),
('ElevatorEntry', 'A player has entered the elevator.', 'X,Y pos – the position of the player before being teleported into the elevator.', 'Almost always neutral'),
('ElevatorExit', 'A player has exited the elevator.', 'X,Y pos – the position of the player before being teleported to the front of the elevator.', 'Likely neutral if the position is the start elevator.'),
('ElevatorOpens', 'How many times the player hit the button to open the elevator doors.', 'Action specific data - the number of times the button was pressed in that floor.', 'Likely altruistic if not all players are in the elevator.'),
('ItemPickUp', 'A Player picked up an interactive item', 'X,Y pos – the position of the player when picking up the item.\nAction specific data - the type of item (E.g. money bag, alien).', 'Likely altruistic if the player picking up the item has the lowest score.\nLikely shellfish if the player with the highest score picks it up.'),
('ItemThrow', 'A Player threw an interactive item.', 'X,Y pos – the player’s position when initiating the throw.\nAction specific data - two items, separated by a comma: the type of item (E.g. money bag, alien) and then the direction of the throw (In degrees, where 0 degrees is directly right).', 'Likely altruistic if the item is the alien & direction of throw is away from any players.\nNeutral if the item is the money bag & direction of throw is towards the end of the level.\nLikely selfish if the item is the alien & the direction of throw is towards another player.\nLikely selfish if the item is the money bag & the direction of throw is towards the start of the level.'),
('Jump', 'A player initiated a jump', 'X,Y pos – the player’s position when initiating the jump.\nAction specific data – the player’s X velocity when initiating the jump.', 'Most likely neutral.'),
('PlatformSpawn', 'The tablet player spawned a new platform in the scene.', 'Timestamp – the time the platform became intractable within the level (finished spawning).\nX,Y pos – the centre point of the platform when spawned.\nAction specific data - the duration (in seconds) the platform was spawned for.', 'Likely selfish if it is placed over the top of another player or not next to an obstacle.\nLikely altruistic if positioned in near an obstacle and another player jumps on it while spawned.'),
('PlayerDeath', 'A player died.', 'X,Y pos – the position of the player when they died.\nAction specific data - the cause of death (E.g. Lazer, alien).', 'Likely selfish if death was the result of being hit by the alien.\nLikely neutral otherwise.'),
('PlayerSpawn', 'A player is spawned / respawned.', 'X,Y pos – the position of the player once spawned.', 'Most likely neutral.');

-- --------------------------------------------------------

--
-- Table structure for table `researchers`
--

CREATE TABLE `researchers` (
  `id` int(11) NOT NULL,
  `name` varchar(100) CHARACTER SET utf8 NOT NULL,
  `email` varchar(100) CHARACTER SET utf8 NOT NULL,
  `password` varchar(100) CHARACTER SET utf8 NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `rounds`
--

CREATE TABLE `rounds` (
  `roundID` int(11) NOT NULL,
  `sessionID` int(11) NOT NULL,
  `sceneName` varchar(255) CHARACTER SET utf8 NOT NULL,
  `roundStartTime` float NOT NULL,
  `player1StartingScore` int(11) NOT NULL,
  `player2StartingScore` int(11) DEFAULT NULL,
  `player3StartingScore` int(11) DEFAULT NULL,
  `player4StartingScore` int(11) DEFAULT NULL,
  `player5StartingScore` int(11) DEFAULT NULL,
  `tabletPlayer` int(1) DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `sessions`
--

CREATE TABLE `sessions` (
  `sessionID` int(11) NOT NULL,
  `sessionStartTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `player1` int(11) NOT NULL,
  `player2` int(11) DEFAULT NULL,
  `player3` int(11) DEFAULT NULL,
  `player4` int(11) DEFAULT NULL,
  `player5` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `id` int(11) NOT NULL,
  `name` varchar(100) CHARACTER SET utf8 NOT NULL,
  `username` varchar(100) CHARACTER SET utf8 NOT NULL,
  `email` varchar(100) CHARACTER SET utf8 NOT NULL,
  `password` varchar(100) CHARACTER SET utf8 NOT NULL,
  `researcher` tinyint(1) NOT NULL DEFAULT '0',
  `institution` varchar(100) DEFAULT NULL,
  `field` varchar(100) DEFAULT NULL,
  `AllowsDataCollection` tinyint(4) NOT NULL DEFAULT '1',
  `Notes` text
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `data`
--
ALTER TABLE `data`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `interactions`
--
ALTER TABLE `interactions`
  ADD PRIMARY KEY (`interactionID`),
  ADD KEY `metricName` (`metricName`),
  ADD KEY `roundID` (`roundID`);

--
-- Indexes for table `metrics`
--
ALTER TABLE `metrics`
  ADD PRIMARY KEY (`metricName`);

--
-- Indexes for table `researchers`
--
ALTER TABLE `researchers`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `rounds`
--
ALTER TABLE `rounds`
  ADD PRIMARY KEY (`roundID`),
  ADD KEY `sessionID` (`sessionID`);

--
-- Indexes for table `sessions`
--
ALTER TABLE `sessions`
  ADD PRIMARY KEY (`sessionID`),
  ADD KEY `player1` (`player1`),
  ADD KEY `player2` (`player2`),
  ADD KEY `player3` (`player3`),
  ADD KEY `player4` (`player4`),
  ADD KEY `player5` (`player5`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `username` (`username`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `interactions`
--
ALTER TABLE `interactions`
  MODIFY `interactionID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2858;
--
-- AUTO_INCREMENT for table `researchers`
--
ALTER TABLE `researchers`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT for table `rounds`
--
ALTER TABLE `rounds`
  MODIFY `roundID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=62;
--
-- AUTO_INCREMENT for table `sessions`
--
ALTER TABLE `sessions`
  MODIFY `sessionID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;
--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=32;
--
-- Constraints for dumped tables
--

--
-- Constraints for table `interactions`
--
ALTER TABLE `interactions`
  ADD CONSTRAINT `metricRef` FOREIGN KEY (`metricName`) REFERENCES `metrics` (`metricName`),
  ADD CONSTRAINT `roundRef` FOREIGN KEY (`roundID`) REFERENCES `rounds` (`roundID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `rounds`
--
ALTER TABLE `rounds`
  ADD CONSTRAINT `sessionRef` FOREIGN KEY (`sessionID`) REFERENCES `sessions` (`sessionID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `sessions`
--
ALTER TABLE `sessions`
  ADD CONSTRAINT `player1ID` FOREIGN KEY (`player1`) REFERENCES `users` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `player2ID` FOREIGN KEY (`player2`) REFERENCES `users` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `player3ID` FOREIGN KEY (`player3`) REFERENCES `users` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `player4ID` FOREIGN KEY (`player4`) REFERENCES `users` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `player5ID` FOREIGN KEY (`player5`) REFERENCES `users` (`id`) ON UPDATE CASCADE;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
