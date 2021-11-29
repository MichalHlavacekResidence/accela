CREATE DATABASE Accela;

USE Accela;

CREATE TABLE Users (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, Firstname Varchar(255), Lastname Varchar(255), Email Varchar(255) NOT NULL, Password Text NOT NULL, Level Varchar(12) NOT NULL);

CREATE TABLE Departments (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, Name varchar(100) NOT NULL, URL text NOT NULL, Visibility boolean NOT NULL, Position int(11) NOT NULL);

CREATE TABLE Managers (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, Firstname Varchar(255) NOT NULL, Lastname Varchar(255) NOT NULL, Email Varchar(255) NOT NULL, Phone Varchar(30), Img Text, Description Text, Visibility boolean NOT NULL, Position int(10) NOT NULL, DepartmentID int(11), FOREIGN KEY (DepartmentID) REFERENCES Departments(ID)); 

CREATE TABLE Brands (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, ContactID int(11) NOT NULL, FOREIGN KEY (ContactID) REFERENCES Managers(ID), Name Varchar(255) NOT NULL, URL Varchar(255) NOT NULL, Description Text, Small_desc Text, Link Text, Position int(10) NOT NULL, Visibility boolean NOT NULL, Image TEXT); 

CREATE TABLE Pools (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, Name Varchar(255) NOT NULL, Url Varchar(255) NOT NULL, Description Text NOT NULL, Visibility boolean NOT NULL, Position int(10) NOT NULL, Img Text, ContactID int(11), FOREIGN KEY (ContactID) REFERENCES Managers(ID)); 

CREATE TABLE Categories (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, PoolID int(11) NOT NULL, FOREIGN KEY (PoolID) REFERENCES Pools(ID), ContactID int(11) NOT NULL, FOREIGN KEY (ContactID) REFERENCES Managers(ID), Name Varchar(255) NOT NULL, Url Varchar(255) NOT NULL, Description Text, Img Text, Visibility boolean NOT NULL, Position int(10) NOT NULL);

CREATE TABLE Products (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, Name Varchar(255) NOT NULL, URL Varchar(255) NOT NULL, Subtitle Text, Small_desc Text, Description Text, Link Text, Visibility boolean NOT NULL, BrandID int(11) NOT NULL, FOREIGN KEY (BrandID) REFERENCES Brands(ID), CategoryID int(11) NOT NULL, FOREIGN KEY (CategoryID) REFERENCES Categories(ID), VideoURL Text, ReferenceLink Text, ManagerID int(11), FOREIGN KEY (ManagerID) REFERENCES Managers(ID), Image TEXT,Position int(10) NOT NULL);

CREATE TABLE ProductAttachements (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, ProductID int(20) NOT NULL, FOREIGN KEY (ProductID) REFERENCES Products(ID), Link Text NOT NULL, Type Varchar(255) NOT NULL);
/*pool brands ne data o brandech se budou brat z produktu*/
CREATE TABLE PoolBrands (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, BrandID int(20) NOT NULL, FOREIGN KEY (BrandID) REFERENCES Brands(ID), PoolID int(11) NOT NULL, FOREIGN KEY (PoolID) REFERENCES Pools(ID), Position int(5) NOT NULL);

CREATE TABLE CustomerMessages (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, Name varchar(255) NOT NULL, Email varchar(255) NOT NULL, Text text NOT NULL, FormURL varchar(255) NOT NULL, Created DateTime DEFAULT CURRENT_TIMESTAMP);
/*ne, pouzivat druhy Tags*/
CREATE TABLE Tags (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, Name VARCHAR(60));

CREATE TABLE Tags (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, Name VARCHAR(60), Url VARCHAR(60),  Position int(5) NOT NULL, Visibility boolean NOT NULL);

CREATE TABLE News (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, 
Title VARCHAR(255) NOT NULL, 
Visibility Boolean,
BrandID int(11) NOT NULL, FOREIGN KEY (BrandID) REFERENCES Brands(ID),
ContactID int(11) NOT NULL, FOREIGN KEY (ContactID) REFERENCES Managers(ID),
ImageBig text,
ImageSmall text, 
Content text NOT NULL, 
ContentSmall text, 
Created Datetime NOT NULL DEFAULT CURRENT_TIMESTAMP, 
Published Datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
VideoURL text,
ButtonText VARCHAR(25),
ButtonUrl text,Perex text,
imageNew text);
---------


-----------

CREATE TABLE NewsTags (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, NewsID int(11) NOT NULL, TagID int(11) NOT NULL, FOREIGN KEY (NewsID) REFERENCES News(ID), FOREIGN KEY (TagID) REFERENCES Tags(ID));

CREATE TABLE ProductNew (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, ProductID int(11) NOT NULL, FOREIGN KEY (ProductID) REFERENCES Products(ID),NewID int(11) NOT NULL, FOREIGN KEY (NewID) REFERENCES News(ID), Position int(5) NOT NULL );

CREATE TABLE Ref (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL,Name varchar(100) NOT NULL,Company text,Img Text,Visibility Boolean,Position int(5) NOT NULL,Description text );

CREATE TABLE NewProducts (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, NewID int(20) NOT NULL, FOREIGN KEY (NewID) REFERENCES News(ID), ProductID int(11) NOT NULL, FOREIGN KEY (ProductID) REFERENCES Products(ID), Position int(5) NOT NULL);

CREATE TABLE NewCategory (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, CategoryID int(11) NOT NULL, FOREIGN KEY (CategoryID) REFERENCES Categories(ID), NewID int(20) NOT NULL, FOREIGN KEY (NewID) REFERENCES News(ID), Position int(5) NOT NULL);

CREATE TABLE NewsTech (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, NewID int(20) NOT NULL, FOREIGN KEY (NewID) REFERENCES News(ID), CategoryID int(11) NOT NULL, FOREIGN KEY (CategoryID) REFERENCES Categories(ID), Position int(5) NULL);

CREATE TABLE ProductRef (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, ProductID int(11) NOT NULL, FOREIGN KEY (ProductID) REFERENCES Products(ID), RefID int(11) NOT NULL, FOREIGN KEY (RefID) REFERENCES Ref(ID), Position int(5) NULL);

CREATE TABLE RelatedProduct (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, ProductID int(11) NOT NULL, FOREIGN KEY (ProductID) REFERENCES Products(ID), RelatedProductID int(11) NOT NULL, FOREIGN KEY (RelatedProductID) REFERENCES Products(ID) , Position int(5) NULL);

//email db

CREATE TABLE EmailTags (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, Name Varchar(255), Visibility boolean NOT NULL );

CREATE TABLE EmailUsers (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, Firstname Varchar(255), Lastname Varchar(255),Email Varchar(255) NOT NULL,  Status  Varchar (255),Address Varchar(255),PhoneNumber Varchar(255),Created DateTime NULL DEFAULT CURRENT_TIMESTAMP,DataChanged Datetime NULL );
 
CREATE TABLE EmailUserTags (ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, EmailTagsID int(11) NOT NULL, FOREIGN KEY (EmailTagsID) REFERENCES EmailTags(ID),EmailUsersID int(11) NOT NULL, FOREIGN KEY (EmailUsersID) REFERENCES EmailUsers(ID),Description Varchar(255));

CREATE TABLE CampaignTypes(ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, Name varchar(100) NOT NULL, Visibility Boolean NOT NULL);
CampaignType(ID, Name,Visibility)

CREATE TABLE EmailCampaigns(ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, CampaignTypesID int(11) NOT NULL, FOREIGN KEY (CampaignTypesID) REFERENCES CampaignTypes(ID), Created Datetime NOT NULL DEFAULT CURRENT_TIMESTAMP, SendetTime Datetime NULL, Sendet Boolean NOT NULL,Name varchar(200) NOT NULL,Subject varchar(200) NOT NULL,EmailFrom varchar(100) NULL,Url varchar(200) NOT NULL)
EmailCampaign(ID,created,sendet(date),sendet(bool),name,subject,emailFrom,url-nazev souboru,Type-jiny graficky styl nobo cokoli jineho)

CREATE TABLE EmailCampaignTags(ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, EmailCampaignsID int(11) NOT NULL, FOREIGN KEY (EmailCampaignsID) REFERENCES EmailCampaigns(ID), EmailTagsID int(11) NOT NULL, FOREIGN KEY (EmailTagsID) REFERENCES EmailTags(ID));
EmailCampaignTags(ID,IDCampaign,IDTag)

CREATE TABLE EmailNews(ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL, NewsID int(11) NOT NULL, FOREIGN KEY (NewsID) REFERENCES News(ID), EmailCampaignsID int(11) NOT NULL, FOREIGN KEY (EmailCampaignsID) REFERENCES EmailCampaigns(ID));
EmailNews(ID,IDNews,IDEmailCampaign)

CREATE TABLE EmailSend(ID int(11) PRIMARY KEY AUTO_INCREMENT NOT NULL,EmailCampaignsID int(11) NOT NULL, FOREIGN KEY (EmailCampaignsID) REFERENCES EmailCampaigns(ID), EmailUsersID int(11) NOT NULL, FOREIGN KEY (EmailUsersID) REFERENCES EmailUsers(ID), Created Datetime NOT NULL DEFAULT CURRENT_TIMESTAMP);
EmailSend(ID,campaignID,EmailUserID,Date)


//end email db