-- phpMyAdmin SQL Dump
-- version 4.5.4.1deb2ubuntu2.1
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Oct 03, 2019 at 09:55 PM
-- Server version: 5.7.27-0ubuntu0.16.04.1
-- PHP Version: 7.0.33-0ubuntu0.16.04.6

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `InformativeWebsite`
--
CREATE DATABASE IF NOT EXISTS `InformativeWebsite` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
USE `InformativeWebsite`;

-- --------------------------------------------------------

--
-- Table structure for table `age_ranges`
--

CREATE TABLE IF NOT EXISTS `age_ranges` (
  `id` int(3) NOT NULL AUTO_INCREMENT,
  `age_range` varchar(255) CHARACTER SET utf8 NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `age_range` (`age_range`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=latin1;

--
-- Dumping data for table `age_ranges`
--

INSERT INTO `age_ranges` (`id`, `age_range`) VALUES
(2, '12-17 years old'),
(3, '18-24 years old'),
(4, '25-34 years old'),
(5, '35-44 years old'),
(6, '45 years or older'),
(7, 'Rather not say'),
(1, 'Under 12 years old');

-- --------------------------------------------------------

--
-- Table structure for table `articles`
--

CREATE TABLE IF NOT EXISTS `articles` (
  `article_id` int(11) NOT NULL AUTO_INCREMENT,
  `article_cat_id` int(11) NOT NULL,
  `article_name` varchar(255) NOT NULL,
  `article_slug` varchar(255) NOT NULL,
  `article_body` text NOT NULL,
  `img_link` varchar(255) DEFAULT NULL,
  `article_state` tinyint(1) NOT NULL DEFAULT '0',
  `article_views` int(11) NOT NULL,
  `article_created` int(11) NOT NULL,
  PRIMARY KEY (`article_id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `click_count`
--

CREATE TABLE IF NOT EXISTS `click_count` (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `page_url` varchar(255) NOT NULL,
  `visits` int(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;

--
-- Dumping data for table `click_count`
--

INSERT INTO `click_count` (`id`, `page_url`, `visits`) VALUES
(1, 'login', 18);

-- --------------------------------------------------------

--
-- Table structure for table `countries`
--

CREATE TABLE IF NOT EXISTS `countries` (
  `id` int(3) NOT NULL AUTO_INCREMENT,
  `country_name` varchar(255) CHARACTER SET utf8 NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `country_name` (`country_name`)
) ENGINE=InnoDB AUTO_INCREMENT=251 DEFAULT CHARSET=latin1;

--
-- Dumping data for table `countries`
--

INSERT INTO `countries` (`id`, `country_name`) VALUES
(1, 'AD - Andorra'),
(2, 'AE - United Arab Emirates'),
(3, 'AF - Afghanistan'),
(4, 'AG - Antigua and Barbuda'),
(5, 'AI - Anguilla'),
(6, 'AL - Albania'),
(7, 'AM - Armenia'),
(8, 'AO - Angola'),
(9, 'AQ - Antarctica'),
(10, 'AR - Argentina'),
(11, 'AS - American Samoa'),
(12, 'AT - Austria),'),
(13, 'AU - Australia'),
(14, 'AW - Aruba'),
(15, 'AZ - Azerbaijan'),
(16, 'BA - Bosnia and Herzegovina'),
(17, 'BB - Barbados'),
(18, 'BD - Bangladesh'),
(19, 'BE - Belgium'),
(20, 'BF - Burkina Faso'),
(21, 'BG - Bulgaria'),
(22, 'BH - Bahrain'),
(23, 'BI - Burundi'),
(24, 'BJ - Benin'),
(25, 'BL - Saint Barthelemy'),
(26, 'BM - Bermuda'),
(27, 'BN - Brunei'),
(28, 'BO - Bolivia'),
(29, 'BR - Brazil'),
(30, 'BS - Bahamas, The'),
(31, 'BT - Bhutan'),
(32, 'BV - Bouvet Island'),
(33, 'BW - Botswana'),
(34, 'BY - Belarus'),
(35, 'BZ - Belize'),
(36, 'CA - Canada'),
(37, 'CC - Cocos (Keeling) Islands'),
(38, 'CD - Congo, Democratic Republic of the'),
(39, 'CF - Central African Republic'),
(40, 'CG - Congo, Republic of the'),
(41, 'CH - Switzerland'),
(42, 'CI - Cote d\'Ivoire'),
(43, 'CK - Cook Islands'),
(44, 'CL - Chile'),
(45, 'CM - Cameroon'),
(46, 'CN - China'),
(47, 'CO - Colombia'),
(48, 'CR - Costa Rica'),
(49, 'CU - Cuba'),
(50, 'CV - Cape Verde'),
(51, 'CW - Curacao'),
(52, 'CX - Christmas Island'),
(53, 'CY - Cyprus'),
(54, 'CZ - Czech Republic'),
(55, 'DE - Germany'),
(56, 'DJ - Djibouti'),
(57, 'DK - Denmark'),
(58, 'DM - Dominica'),
(59, 'DO - Dominican Republic'),
(60, 'DZ - Algeria'),
(61, 'EC - Ecuador'),
(62, 'EE - Estonia'),
(63, 'EG - Egypt'),
(64, 'EH - Western Sahara'),
(65, 'ER - Eritrea'),
(66, 'ES - Spain'),
(67, 'ET - Ethiopia'),
(68, 'FI - Finland'),
(69, 'FJ - Fiji'),
(70, 'FK - Falkland Islands (Islas Malvinas)'),
(71, 'FM - Micronesia, Federated States of'),
(72, 'FO - Faroe Islands'),
(73, 'FR - France'),
(74, 'FX - France, Metropolitan'),
(75, 'GA - Gabon'),
(76, 'GB - United Kingdom'),
(77, 'GD - Grenada'),
(78, 'GE - Georgia'),
(79, 'GF - French Guiana'),
(80, 'GG - Guernsey'),
(81, 'GH - Ghana'),
(82, 'GI - Gibraltar'),
(83, 'GL - Greenland'),
(84, 'GM - Gambia, The'),
(85, 'GN - Guinea'),
(86, 'GP - Guadeloupe'),
(87, 'GQ - Equatorial Guinea'),
(88, 'GR - Greece'),
(89, 'GS - South Georgia and the Islands'),
(90, 'GT - Guatemala'),
(91, 'GU - Guam'),
(92, 'GW - Guinea-Bissau'),
(93, 'GY - Guyana'),
(94, 'HK - Hong Kong'),
(95, 'HM - Heard Island and McDonald Islands'),
(96, 'HN - Honduras'),
(97, 'HR - Croatia'),
(98, 'HT - Haiti'),
(99, 'HU - Hungary'),
(100, 'ID - Indonesia'),
(101, 'IE - Ireland'),
(102, 'IL - Israel'),
(103, 'IM - Isle of Man'),
(104, 'IN - India'),
(105, 'IO - British Indian Ocean Territory'),
(106, 'IQ - Iraq'),
(107, 'IR - Iran'),
(108, 'IS - Iceland'),
(109, 'IT - Italy'),
(110, 'JE - Jersey'),
(111, 'JM - Jamaica'),
(112, 'JO - Jordan'),
(113, 'JP - Japan'),
(114, 'KE - Kenya'),
(115, 'KG - Kyrgyzstan'),
(116, 'KH - Cambodia'),
(117, 'KI - Kiribati'),
(118, 'KM - Comoros'),
(119, 'KN - Saint Kitts and Nevis'),
(120, 'KP - Korea, North'),
(121, 'KR - Korea, South'),
(122, 'KW - Kuwait'),
(123, 'KY - Cayman Islands'),
(124, 'KZ - Kazakhstan'),
(125, 'LA - Laos'),
(126, 'LB - Lebanon'),
(127, 'LC - Saint Lucia'),
(128, 'LI - Liechtenstein'),
(129, 'LK - Sri Lanka'),
(130, 'LR - Liberia'),
(131, 'LS - Lesotho'),
(132, 'LT - Lithuania'),
(133, 'LU - Luxembourg'),
(134, 'LV - Latvia'),
(135, 'LY - Libya'),
(136, 'MA - Morocco'),
(137, 'MC - Monaco'),
(138, 'MD - Moldova'),
(139, 'ME - Montenegro'),
(140, 'MF - Saint Martin'),
(141, 'MG - Madagascar'),
(142, 'MH - Marshall Islands'),
(143, 'MK - Macedonia'),
(144, 'ML - Mali'),
(145, 'MM - Burma'),
(146, 'MN - Mongolia'),
(147, 'MO - Macau'),
(148, 'MP - Northern Mariana Islands'),
(149, 'MQ - Martinique'),
(150, 'MR - Mauritania'),
(151, 'MS - Montserrat'),
(152, 'MT - Malta'),
(153, 'MU - Mauritius'),
(154, 'MV - Maldives'),
(155, 'MW - Malawi'),
(156, 'MX - Mexico'),
(157, 'MY - Malaysia'),
(158, 'MZ - Mozambique'),
(159, 'NA - Namibia'),
(160, 'NC - New Caledonia'),
(161, 'NE - Niger'),
(162, 'NF - Norfolk Island'),
(163, 'NG - Nigeria'),
(164, 'NI - Nicaragua'),
(165, 'NL - Netherlands'),
(166, 'NO - Norway'),
(167, 'NP - Nepal'),
(168, 'NR - Nauru'),
(169, 'NU - Niue'),
(170, 'NZ - New Zealand'),
(171, 'OM - Oman'),
(172, 'PA - Panama'),
(173, 'PE - Peru'),
(174, 'PF - French Polynesia'),
(175, 'PG - Papua New Guinea'),
(176, 'PH - Philippines'),
(177, 'PK - Pakistan'),
(178, 'PL - Poland'),
(179, 'PM - Saint Pierre and Miquelon'),
(180, 'PN - Pitcairn Islands'),
(181, 'PR - Puerto Rico'),
(182, 'PS - Gaza Strip'),
(183, 'PS - West Bank'),
(184, 'PT - Portugal'),
(185, 'PW - Palau'),
(186, 'PY - Paraguay'),
(187, 'QA - Qatar'),
(188, 'RE - Reunion'),
(189, 'RO - Romania'),
(190, 'RS - Serbia'),
(191, 'RU - Russia'),
(192, 'RW - Rwanda'),
(193, 'SA - Saudi Arabia'),
(194, 'SB - Solomon Islands'),
(195, 'SC - Seychelles'),
(196, 'SD - Sudan'),
(197, 'SE - Sweden'),
(198, 'SG - Singapore'),
(199, 'SH - Saint Helena, Ascension, and Tristan da Cunha'),
(200, 'SI - Slovenia'),
(201, 'SJ - Svalbard'),
(202, 'SK - Slovakia'),
(203, 'SL - Sierra Leone'),
(204, 'SM - San Marino'),
(205, 'SN - Senegal'),
(206, 'SO - Somalia'),
(207, 'SR - Suriname'),
(208, 'SS - South Sudan'),
(209, 'ST - Sao Tome and Principe'),
(210, 'SV - El Salvador'),
(211, 'SX - Sint Maarten'),
(212, 'SY - Syria'),
(213, 'SZ - Swaziland'),
(214, 'TC - Turks and Caicos Islands'),
(215, 'TD - Chad'),
(216, 'TF - French Southern and Antarctic Lands'),
(217, 'TG - Togo'),
(218, 'TH - Thailand'),
(219, 'TJ - Tajikistan'),
(220, 'TK - Tokelau'),
(221, 'TL - Timor-Leste'),
(222, 'TM - Turkmenistan'),
(223, 'TN - Tunisia'),
(224, 'TO - Tonga'),
(225, 'TR - Turkey'),
(226, 'TT - Trinidad and Tobago'),
(227, 'TV - Tuvalu'),
(228, 'TW - Taiwan'),
(229, 'TZ - Tanzania'),
(230, 'UA - Ukraine'),
(231, 'UG - Uganda'),
(232, 'UM - United States Minor Outlying Islands'),
(233, 'US - United States'),
(234, 'UY - Uruguay'),
(235, 'UZ - Uzbekistan'),
(236, 'VA - Holy See (Vatican City)'),
(237, 'VC - Saint Vincent and the Grenadines'),
(238, 'VE - Venezuela'),
(239, 'VG - British Virgin Islands'),
(240, 'VI - Virgin Islands'),
(241, 'VN - Vietnam'),
(242, 'VU - Vanuatu'),
(243, 'WF - Wallis and Futuna'),
(244, 'WS - Samoa'),
(245, 'XK - Kosovo'),
(246, 'YE - Yemen'),
(247, 'YT - Mayotte'),
(248, 'ZA - South Africa'),
(249, 'ZM - Zambia'),
(250, 'ZW - Zimbabwe');

-- --------------------------------------------------------

--
-- Table structure for table `data`
--

CREATE TABLE IF NOT EXISTS `data` (
  `id` int(1) NOT NULL,
  `user_id` int(11) NOT NULL,
  `collisions` int(11) NOT NULL,
  `score` int(11) NOT NULL,
  `visits` int(15) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `data`
--

INSERT INTO `data` (`id`, `user_id`, `collisions`, `score`, `visits`) VALUES
(1, 1, 3, 2350, 0),
(2, 7, 18, 230, 0);

-- --------------------------------------------------------

--
-- Table structure for table `expressions_of_interest`
--

CREATE TABLE IF NOT EXISTS `expressions_of_interest` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `submission_time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `email` varchar(255) NOT NULL,
  `age_range` int(11) NOT NULL,
  `country` int(11) NOT NULL,
  `early_access` tinyint(4) NOT NULL DEFAULT '0',
  `comment` text,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;

--
-- Dumping data for table `expressions_of_interest`
--

INSERT INTO `expressions_of_interest` (`id`, `submission_time`, `email`, `age_range`, `country`, `early_access`, `comment`) VALUES
(1, '2019-10-03 21:52:34', 'cameron@cameron.cameron', 1, 17, 1, 'how did i make this?'),
(2, '2019-10-03 21:53:46', 'legitPineapple@dodo.com.au', 4, 13, 0, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `express_interest_clicks`
--

CREATE TABLE IF NOT EXISTS `express_interest_clicks` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `click_time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=latin1;

--
-- Dumping data for table `express_interest_clicks`
--

INSERT INTO `express_interest_clicks` (`id`, `click_time`) VALUES
(1, '2019-10-03 21:35:22'),
(2, '2019-10-03 21:35:25'),
(3, '2019-10-03 21:35:28'),
(4, '2019-10-03 21:36:18'),
(5, '2019-10-03 21:53:58'),
(6, '2019-10-03 21:54:00'),
(7, '2019-10-03 21:54:01'),
(8, '2019-10-03 21:54:04');

-- --------------------------------------------------------

--
-- Table structure for table `page_visits`
--

CREATE TABLE IF NOT EXISTS `page_visits` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `page_visited` int(3) NOT NULL,
  `visit_time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=latin1;

--
-- Dumping data for table `page_visits`
--

INSERT INTO `page_visits` (`id`, `page_visited`, `visit_time`) VALUES
(1, 1, '2019-10-03 21:31:59'),
(2, 2, '2019-10-03 21:32:05'),
(3, 3, '2019-10-03 21:32:10'),
(4, 3, '2019-10-03 21:32:13'),
(5, 3, '2019-10-03 21:32:16'),
(6, 3, '2019-10-03 21:32:19'),
(7, 3, '2019-10-03 21:35:40');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE IF NOT EXISTS `users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(100) CHARACTER SET utf8 NOT NULL,
  `username` varchar(100) CHARACTER SET utf8 NOT NULL,
  `email` varchar(100) CHARACTER SET utf8 NOT NULL,
  `password` varchar(100) CHARACTER SET utf8 NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `username` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=latin1;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`id`, `name`, `username`, `email`, `password`) VALUES
(1, 'test', 'tester', 'test@123.com', 'test123'),
(2, 'Anthony', 'Antonio', 'ant@comcast.net', '$2y$10$.yzVu78gqh9C8xiuIFCT4ucYl7Z.IF1hvrPbz61KccVDeqd4MaOU6'),
(3, 'eawr', 'wearaew', 'howard@gmail.com', '$2y$10$1DwElEbFuWcV0Dd5yOdiYek7Ca5b3T4dlnOp3KRR0YFxrPo2KcujG'),
(4, 'Howard', 'Howard', 'howard1@gmail.com', '$2y$10$LJe3wQJitXmukZlIG4VnduoT1w9dysj8LICg/WKWV0EY4GB5WHcua'),
(5, 'Cameron', 'Cameron', 'cameron@gmail.com', '$2y$10$ALkbFtZfOQC0ppQOY8g.EOjurOi1i.p2JWlKe/UGWTyUcK8eoZkaG'),
(14, 'Cameron7', 'Cameron7', 'cameron7@gmail.com', '$2y$10$w.uk.YO54Q4av0mjgDSikO/EgbTJJPtBCCJFCB3A5khpnDjtc80GC');

-- --------------------------------------------------------

--
-- Table structure for table `website_pages`
--

CREATE TABLE IF NOT EXISTS `website_pages` (
  `id` int(3) NOT NULL AUTO_INCREMENT,
  `page_name` varchar(255) CHARACTER SET utf8 NOT NULL,
  `url` varchar(255) CHARACTER SET utf8 NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `page_name` (`page_name`),
  UNIQUE KEY `url` (`url`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;

--
-- Dumping data for table `website_pages`
--

INSERT INTO `website_pages` (`id`, `page_name`, `url`) VALUES
(1, 'Landing Page', 'home'),
(2, 'Express Interest Page', 'users'),
(3, 'Administrator Page', 'data');

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
