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
-- Table structure for table `kittransactions`
--

DROP TABLE IF EXISTS `kittransactions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `kittransactions` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Beneficiary_ID` varchar(50) DEFAULT NULL,
  `Terminal_ID` varchar(50) DEFAULT NULL,
  `Month` varchar(20) DEFAULT NULL,
  `Date` date DEFAULT NULL,
  `Amount` decimal(10,2) DEFAULT '100.00',
  `Status` enum('Received','Pending') DEFAULT 'Received',
  `Create_Date` datetime DEFAULT CURRENT_TIMESTAMP,
  `Update_Date` datetime DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `fk_beneficiary` (`Beneficiary_ID`),
  KEY `fk_terminal` (`Terminal_ID`),
  CONSTRAINT `fk_beneficiary` FOREIGN KEY (`Beneficiary_ID`) REFERENCES `beneficiaries` (`Beneficiary_ID`),
  CONSTRAINT `fk_terminal` FOREIGN KEY (`Terminal_ID`) REFERENCES `terminalusers` (`Terminal_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `kittransactions`
--

LOCK TABLES `kittransactions` WRITE;
/*!40000 ALTER TABLE `kittransactions` DISABLE KEYS */;
INSERT INTO `kittransactions` VALUES (1,'BENb92b24','TERM001','January','2026-01-31',100.00,'Received','2026-01-31 17:50:22',NULL),(2,'BENb68744','TERM001','February','2026-02-01',100.00,'Received','2026-02-01 16:12:46',NULL),(3,'BENb92b24','TERM001','February','2026-02-01',100.00,'Received','2026-02-01 16:13:10',NULL),(4,'BENb68744','TERM001','March','2026-03-04',100.00,'Received','2026-02-03 13:33:55',NULL);
/*!40000 ALTER TABLE `kittransactions` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-02-05 10:31:58
