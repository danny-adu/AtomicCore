﻿USE [master]
GO
/****** Object:  Database [DLYS_MNKS]    Script Date: 2023/1/18 22:50:05 ******/
CREATE DATABASE [DLYS_MNKS]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DLYS_MNKS', FILENAME = N'/var/opt/mssql/data/DLYS_MNKS.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'DLYS_MNKS_log', FILENAME = N'/var/opt/mssql/data/DLYS_MNKS_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [DLYS_MNKS] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DLYS_MNKS].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DLYS_MNKS] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DLYS_MNKS] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DLYS_MNKS] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DLYS_MNKS] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DLYS_MNKS] SET ARITHABORT OFF 
GO
ALTER DATABASE [DLYS_MNKS] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [DLYS_MNKS] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [DLYS_MNKS] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DLYS_MNKS] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DLYS_MNKS] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DLYS_MNKS] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DLYS_MNKS] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DLYS_MNKS] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DLYS_MNKS] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DLYS_MNKS] SET  DISABLE_BROKER 
GO
ALTER DATABASE [DLYS_MNKS] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DLYS_MNKS] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DLYS_MNKS] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DLYS_MNKS] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DLYS_MNKS] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DLYS_MNKS] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [DLYS_MNKS] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DLYS_MNKS] SET RECOVERY FULL 
GO
ALTER DATABASE [DLYS_MNKS] SET  MULTI_USER 
GO
ALTER DATABASE [DLYS_MNKS] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DLYS_MNKS] SET DB_CHAINING OFF 
GO
ALTER DATABASE [DLYS_MNKS] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [DLYS_MNKS] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [DLYS_MNKS] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [DLYS_MNKS] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'DLYS_MNKS', N'ON'
GO
ALTER DATABASE [DLYS_MNKS] SET QUERY_STORE = OFF
GO
USE [DLYS_MNKS]
GO
/****** Object:  Table [dbo].[Topic_QQS]    Script Date: 2023/1/18 22:50:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Topic_QQS](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Sex] [int] NOT NULL,
	[QQ] [varchar](50) NOT NULL,
	[IsDel] [int] NOT NULL,
 CONSTRAINT [PK_Topic_QQS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Topic_QQS_Ext]    Script Date: 2023/1/18 22:50:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Topic_QQS_Ext](
	[ID] [int] NOT NULL,
	[Reamark] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Topic_QQS_Ext] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
USE [master]
GO
ALTER DATABASE [DLYS_MNKS] SET  READ_WRITE 
GO
