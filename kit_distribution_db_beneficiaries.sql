-- MySQL dump 10.13  Distrib 8.0.45, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: kit_distribution_db
-- ------------------------------------------------------
-- Server version	8.0.45

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `beneficiaries`
--

DROP TABLE IF EXISTS `beneficiaries`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `beneficiaries` (
  `Beneficiary_ID` varchar(50) NOT NULL,
  `Card_ID` varchar(100) DEFAULT NULL,
  `FullName` varchar(100) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `Mobile` varchar(15) DEFAULT NULL,
  `State_City` varchar(100) DEFAULT NULL,
  `Status` enum('Active','Inactive') DEFAULT 'Active',
  `Create_Date` datetime DEFAULT CURRENT_TIMESTAMP,
  `Update_Date` datetime DEFAULT NULL,
  `UpdateDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Beneficiary_ID`),
  UNIQUE KEY `Card_ID` (`Card_ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `beneficiaries`
--

LOCK TABLES `beneficiaries` WRITE;
/*!40000 ALTER TABLE `beneficiaries` DISABLE KEYS */;
INSERT INTO `beneficiaries` VALUES ('BENb68744','QR_BENb68744.png','FATEH','$2a$11$zCihE6lfrMBF9vkifEat/.KJD6mmRF7EdpzQp8KdzNSEmlRfE4MDW','9265462429','Palanpur','Active','2026-02-01 15:50:57',NULL,'2026-02-03 13:43:18'),('BENb92b24','QR_BENb92b24.png','Fattehali','$2a$11$Sp6tRQ5IBQCvYegwERZiF.4Dc20Z86njXLsnOOjpIIWiAbKpbR1aq','7984897033','Palanpur','Active','2026-01-31 17:48:01',NULL,'2026-01-31 18:13:10');
/*!40000 ALTER TABLE `beneficiaries` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-02-05 10:31:59
