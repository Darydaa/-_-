--use master;
--create database InfoMovie;
use InfoMovie;
--create table Users
--( Login nvarchar(100) constraint Users_PK primary key,
--Administrator bit ,
--Password nvarchar(100)
--);
--create table Comments(
-- Id int IDENTITY(1,1) constraint Comments_FK1 foreign key
--                   references Content(Id), 
-- User_comm nvarchar(100) constraint Comments_FK foreign key
--                   references Users(Login), 
--Text nvarchar(250),
--Id_content int,
--Date smalldatetime
--);
--create table Search_History(
--Id int IDENTITY(1,1) constraint Search_History_PK primary key,
--Text_Of_Search nvarchar(100),
--Type nvarchar(30),
--User_search nvarchar (100) constraint Search_History_FK foreign key
--                   references Users(Login), 


--);
--create table Content_Answer(
--Search_Id int constraint Content_Answer_FK1 foreign key
--                   references Search_History(Id), 
--Content_Id int constraint Content_Answer_FK foreign key
--                   references Content(Id)

--);
--create table Content(
--Id int IDENTITY(1,1) constraint Content_PK primary key,
--Genre_ids nvarchar(100),
--Popularity float,
--Vote_count int,
-- Backdrop_path nvarchar(100),
-- Original_language nvarchar(100),
-- Vote_average float,
-- Overview nvarchar(500),
-- Poster_path nvarchar(100),
-- Adult bit,
-- Release_date nvarchar(100),
-- Original_title nvarchar(100),
-- Title nvarchar(100),
-- Video bit,
-- Original_name nvarchar(100),
-- Name nvarchar(100),
-- Origin_country nvarchar(100),
-- First_air_date nvarchar(100)

--);
--create table Genres(
--Id int  constraint Genres_PK primary key,
--Name nvarchar(100)

--)