/*
Navicat MySQL Data Transfer

Source Server         : localhost_3306
Source Server Version : 50540
Source Host           : localhost:3306
Source Database       : darkgod

Target Server Type    : MYSQL
Target Server Version : 50540
File Encoding         : 65001

Date: 2019-06-11 18:44:53
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `account`
-- ----------------------------
DROP TABLE IF EXISTS `account`;
CREATE TABLE `account` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `acct` varchar(255) NOT NULL,
  `pwd` varchar(255) NOT NULL,
  `name` varchar(255) NOT NULL,
  `level` int(11) NOT NULL,
  `exp` int(11) NOT NULL,
  `power` int(11) NOT NULL,
  `coin` int(11) NOT NULL,
  `diamond` int(11) NOT NULL,
  `crystal` int(11) NOT NULL,
  `hp` int(11) NOT NULL,
  `ad` int(11) NOT NULL,
  `ap` int(11) NOT NULL,
  `addef` int(11) NOT NULL,
  `apdef` int(11) NOT NULL,
  `dodge` int(11) NOT NULL,
  `pierce` int(11) NOT NULL,
  `critical` int(11) NOT NULL,
  `guideid` int(11) NOT NULL DEFAULT '1001',
  `strong` varchar(255) NOT NULL,
  `time` bigint(20) NOT NULL,
  `task` varchar(255) NOT NULL,
  `dungeon` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of account
-- ----------------------------
INSERT INTO `account` VALUES ('20', '1', '1', '孙妃', '9', '2500', '296', '30448', '400', '962', '2466', '809', '799', '504', '480', '7', '5', '2', '1006', '2#9#2#2#2#2#', '1560246031383', '1|1|1#2|3|1#3|5|1#4|2|1#5|5|1#6|3|1#', '10007');
INSERT INTO `account` VALUES ('21', '2', '1', '夏侯晶', '5', '1260', '240', '9742', '470', '485', '2206', '514', '504', '249', '225', '7', '5', '2', '1007', '1#4#1#1#1#1#', '1560225974059', '1|1|1#2|1|0#3|5|1#4|1|0#5|2|0#6|1|0#', '10002');
INSERT INTO `account` VALUES ('22', '3', '1', '姜七', '3', '750', '150', '5980', '500', '548', '2000', '275', '265', '67', '43', '7', '5', '2', '1001', '0#0#0#0#0#0#', '1560229565076', '1|0|0#2|1|0#3|0|0#4|0|0#5|0|0#6|0|0#', '10002');
