CREATE DATABASE Accela;

USE Accela;

CREATE TABLE Users (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, Firstname Varchar(255), Lastname Varchar(255), Email Varchar(255) NOT NULL, Password Text NOT NULL, Level Varchar(12) NOT NULL);

CREATE TABLE Departments (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, Name varchar(100) NOT NULL, URL text NOT NULL, Visibility boolean NOT NULL, Position int(11) NOT NULL);

CREATE TABLE Managers (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, Firstname Varchar(255) NOT NULL, Lastname Varchar(255) NOT NULL, Email Varchar(255) NOT NULL, Phone Varchar(30), Img Text, Description Text, Visibility boolean NOT NULL, Position int(10) NOT NULL, DepartmentID int(11), FOREIGN KEY (DepartmentID) REFERENCES Departments(ID)); 

CREATE TABLE Brands (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, ContactID int(11) NOT NULL, FOREIGN KEY (ContactID) REFERENCES Managers(ID), Name Varchar(255) NOT NULL, URL Varchar(255) NOT NULL, Description Text, Small_desc Text, Link Text, Position int(10) NOT NULL, Visibility boolean NOT NULL, Image TEXT); 

CREATE TABLE Pools (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, Name Varchar(255) NOT NULL, Url Varchar(255) NOT NULL, Description Text NOT NULL, Visibility boolean NOT NULL, Position int(10) NOT NULL, Img Text, ContactID int(11), FOREIGN KEY (ContactID) REFERENCES Managers(ID)); 

CREATE TABLE Categories (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, PoolID int(11) NOT NULL, FOREIGN KEY (PoolID) REFERENCES Pools(ID), ContactID int(11) NOT NULL, FOREIGN KEY (ContactID) REFERENCES Managers(ID), Name Varchar(255) NOT NULL, Url Varchar(255) NOT NULL, Description Text, Img Text, Visibility boolean NOT NULL, Position int(10) NOT NULL);

CREATE TABLE Products (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, Name Varchar(255) NOT NULL, URL Varchar(255) NOT NULL, Subtitle Text, Small_desc Text, Description Text, Link Text, Visibility boolean NOT NULL, BrandID int(11) NOT NULL, FOREIGN KEY (BrandID) REFERENCES Brands(ID), CategoryID int(11) NOT NULL, FOREIGN KEY (CategoryID) REFERENCES Categories(ID), VideoURL Text, ReferenceLink Text, ManagerID int(11), FOREIGN KEY (ManagerID) REFERENCES Managers(ID), Image TEXT);

CREATE TABLE ProductAttachements (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, ProductID int(20) NOT NULL, FOREIGN KEY (ProductID) REFERENCES Products(ID), Link Text NOT NULL, Type Varchar(255) NOT NULL);

CREATE TABLE PoolBrands (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, BrandID int(20) NOT NULL, FOREIGN KEY (BrandID) REFERENCES Brands(ID), PoolID int(11) NOT NULL, FOREIGN KEY (PoolID) REFERENCES Pools(ID), Position int(5) NOT NULL);

CREATE TABLE CustomerMessages (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, Name varchar(255) NOT NULL, Email varchar(255) NOT NULL, Text text NOT NULL, FormURL varchar(255) NOT NULL, Created DateTime DEFAULT CURRENT_TIMESTAMP);

CREATE TABLE Tags (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, Name VARCHAR(60));


