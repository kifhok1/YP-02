-- MySQL dump 10.13  Distrib 8.0.43, for Win64 (x86_64)
--
-- Host: localhost    Database: vkr
-- ------------------------------------------------------
-- Server version	9.4.0

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
-- Table structure for table `categoryproduct`
--

DROP TABLE IF EXISTS `categoryproduct`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `categoryproduct` (
  `ID_CategoryProduct` int NOT NULL AUTO_INCREMENT,
  `CategoryProduct` varchar(40) NOT NULL,
  PRIMARY KEY (`ID_CategoryProduct`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `clients`
--

DROP TABLE IF EXISTS `clients`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `clients` (
  `ID_Clients` int NOT NULL AUTO_INCREMENT,
  `Fio` varchar(200) NOT NULL,
  `PhoneNumber` varchar(20) NOT NULL,
  `AmountOfNumber` decimal(11,2) NOT NULL,
  PRIMARY KEY (`ID_Clients`)
) ENGINE=InnoDB AUTO_INCREMENT=55 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `order`
--

DROP TABLE IF EXISTS `order`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `order` (
  `ID_Order` int NOT NULL AUTO_INCREMENT,
  `ID_Client` int NOT NULL,
  `CostOrder` decimal(10,2) NOT NULL,
  `Status` int NOT NULL,
  `Code` varchar(3) NOT NULL,
  `TotalDiscount` int NOT NULL,
  `DateOrder` datetime NOT NULL,
  `DeliveryDate` datetime NOT NULL,
  PRIMARY KEY (`ID_Order`),
  KEY `ID_Client` (`ID_Client`),
  KEY `order_ibfk_1_idx` (`Status`),
  CONSTRAINT `order_ibfk_1` FOREIGN KEY (`ID_Client`) REFERENCES `clients` (`ID_Clients`),
  CONSTRAINT `order_ibfk_2` FOREIGN KEY (`Status`) REFERENCES `orderstatus` (`Id_OrderStatus`)
) ENGINE=InnoDB AUTO_INCREMENT=105 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `orderproduct`
--

DROP TABLE IF EXISTS `orderproduct`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orderproduct` (
  `ID_Order` int NOT NULL,
  `ID_Product` int NOT NULL,
  `Count` int NOT NULL,
  `CostProduct` decimal(9,2) DEFAULT NULL,
  PRIMARY KEY (`ID_Order`,`ID_Product`),
  KEY `ID_Product` (`ID_Product`),
  CONSTRAINT `orderproduct_ibfk_1` FOREIGN KEY (`ID_Order`) REFERENCES `order` (`ID_Order`),
  CONSTRAINT `orderproduct_ibfk_2` FOREIGN KEY (`ID_Product`) REFERENCES `product` (`ID_Product`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `orderstatus`
--

DROP TABLE IF EXISTS `orderstatus`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orderstatus` (
  `Id_OrderStatus` int NOT NULL AUTO_INCREMENT,
  `NameStatus` varchar(45) NOT NULL,
  PRIMARY KEY (`Id_OrderStatus`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `product`
--

DROP TABLE IF EXISTS `product`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `product` (
  `ID_Product` int NOT NULL AUTO_INCREMENT,
  `ProductName` varchar(200) NOT NULL,
  `ProductCategory` int NOT NULL,
  `ProductCountInStock` tinyint NOT NULL,
  `ProductDiscount` tinyint NOT NULL,
  `ProductCost` decimal(9,2) NOT NULL,
  `ProductImage` mediumblob,
  PRIMARY KEY (`ID_Product`),
  KEY `ProductCategory` (`ProductCategory`),
  CONSTRAINT `product_ibfk_1` FOREIGN KEY (`ProductCategory`) REFERENCES `categoryproduct` (`ID_CategoryProduct`)
) ENGINE=InnoDB AUTO_INCREMENT=349 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `rule`
--

DROP TABLE IF EXISTS `rule`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rule` (
  `ID_Rule` int NOT NULL AUTO_INCREMENT,
  `Rule` varchar(20) NOT NULL,
  PRIMARY KEY (`ID_Rule`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `workers`
--

DROP TABLE IF EXISTS `workers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `workers` (
  `ID_Worker` int NOT NULL AUTO_INCREMENT,
  `Fio` varchar(200) NOT NULL,
  `PhoneNumber` varchar(20) NOT NULL,
  `Login` varchar(30) NOT NULL,
  `Password` varchar(100) NOT NULL,
  `rule` int NOT NULL,
  `Image` mediumblob,
  PRIMARY KEY (`ID_Worker`),
  KEY `rule` (`rule`),
  CONSTRAINT `workers_ibfk_1` FOREIGN KEY (`rule`) REFERENCES `rule` (`ID_Rule`)
) ENGINE=InnoDB AUTO_INCREMENT=59 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `categoryproduct`
--

LOCK TABLES `categoryproduct` WRITE;
/*!40000 ALTER TABLE `categoryproduct` DISABLE KEYS */;
INSERT INTO `categoryproduct` VALUES (1,'Зеркальные фотоаппараты'),(2,'Беззеркальные фотоаппараты'),(3,'Объективы'),(4,'Вспышки и свет'),(5,'Штативы и стабилизация'),(6,'Карты памяти и хранилища'),(7,'Аккумуляторы и питание'),(8,'Фотосумки и аксессуары');
/*!40000 ALTER TABLE `categoryproduct` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping data for table `orderstatus`
--

LOCK TABLES `orderstatus` WRITE;
/*!40000 ALTER TABLE `orderstatus` DISABLE KEYS */;
INSERT INTO `orderstatus` VALUES (1,'Доставленно'),(2,'Отправлено'),(3,'Отменен');
/*!40000 ALTER TABLE `orderstatus` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping data for table `rule`
--

LOCK TABLES `rule` WRITE;
/*!40000 ALTER TABLE `rule` DISABLE KEYS */;
INSERT INTO `rule` VALUES (1,'Администратор'),(2,'Продавецц'),(3,'Консультант');
/*!40000 ALTER TABLE `rule` ENABLE KEYS */;
UNLOCK TABLES;

/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-12-14 22:45:43
