
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 03/12/2023 16:38:50
-- Generated from EDMX file: E:\university\WPF\WPF_DB_diagram.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [WPF_DB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_clientsorders]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[orders] DROP CONSTRAINT [FK_clientsorders];
GO
IF OBJECT_ID(N'[dbo].[FK_clients_inherits_person]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[people_clients] DROP CONSTRAINT [FK_clients_inherits_person];
GO
IF OBJECT_ID(N'[dbo].[FK_directors_inherits_person]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[people_directors] DROP CONSTRAINT [FK_directors_inherits_person];
GO
IF OBJECT_ID(N'[dbo].[FK_actors_inherits_person]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[people_actors] DROP CONSTRAINT [FK_actors_inherits_person];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[movies]', 'U') IS NOT NULL
    DROP TABLE [dbo].[movies];
GO
IF OBJECT_ID(N'[dbo].[orders]', 'U') IS NOT NULL
    DROP TABLE [dbo].[orders];
GO
IF OBJECT_ID(N'[dbo].[langs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[langs];
GO
IF OBJECT_ID(N'[dbo].[formats]', 'U') IS NOT NULL
    DROP TABLE [dbo].[formats];
GO
IF OBJECT_ID(N'[dbo].[countries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[countries];
GO
IF OBJECT_ID(N'[dbo].[people]', 'U') IS NOT NULL
    DROP TABLE [dbo].[people];
GO
IF OBJECT_ID(N'[dbo].[people_clients]', 'U') IS NOT NULL
    DROP TABLE [dbo].[people_clients];
GO
IF OBJECT_ID(N'[dbo].[people_directors]', 'U') IS NOT NULL
    DROP TABLE [dbo].[people_directors];
GO
IF OBJECT_ID(N'[dbo].[people_actors]', 'U') IS NOT NULL
    DROP TABLE [dbo].[people_actors];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'movies'
CREATE TABLE [dbo].[movies] (
    [movie_id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL,
    [year] smallint  NULL,
    [country_id] int  NOT NULL,
    [duration] smallint  NOT NULL,
    [age] tinyint  NULL,
    [total_count] tinyint  NOT NULL,
    [price] decimal(18,0)  NOT NULL,
    [left_count] tinyint  NOT NULL,
    [plot] nvarchar(max)  NULL,
    [lang_id] int  NOT NULL,
    [actor_id] int  NOT NULL,
    [director_id] int  NOT NULL,
    [format_id] int  NOT NULL
);
GO

-- Creating table 'orders'
CREATE TABLE [dbo].[orders] (
    [order_id] int IDENTITY(1,1) NOT NULL,
    [client_id] int  NOT NULL,
    [movie_id] int  NOT NULL,
    [rent_date] datetime  NOT NULL,
    [due_date] datetime  NOT NULL,
    [return_date] datetime  NOT NULL
);
GO

-- Creating table 'langs'
CREATE TABLE [dbo].[langs] (
    [lang_id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'formats'
CREATE TABLE [dbo].[formats] (
    [format_id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'countries'
CREATE TABLE [dbo].[countries] (
    [country_id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'people'
CREATE TABLE [dbo].[people] (
    [person_id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NULL,
    [last_name] nvarchar(max)  NOT NULL,
    [gender] nvarchar(max)  NULL
);
GO

-- Creating table 'people_clients'
CREATE TABLE [dbo].[people_clients] (
    [client_id] int IDENTITY(1,1) NOT NULL,
    [phone] nvarchar(max)  NULL,
    [email] nvarchar(max)  NULL,
    [person_id] int  NOT NULL
);
GO

-- Creating table 'people_directors'
CREATE TABLE [dbo].[people_directors] (
    [director_id] int IDENTITY(1,1) NOT NULL,
    [person_id] int  NOT NULL
);
GO

-- Creating table 'people_actors'
CREATE TABLE [dbo].[people_actors] (
    [actor_id] int IDENTITY(1,1) NOT NULL,
    [person_id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [movie_id] in table 'movies'
ALTER TABLE [dbo].[movies]
ADD CONSTRAINT [PK_movies]
    PRIMARY KEY CLUSTERED ([movie_id] ASC);
GO

-- Creating primary key on [order_id] in table 'orders'
ALTER TABLE [dbo].[orders]
ADD CONSTRAINT [PK_orders]
    PRIMARY KEY CLUSTERED ([order_id] ASC);
GO

-- Creating primary key on [lang_id] in table 'langs'
ALTER TABLE [dbo].[langs]
ADD CONSTRAINT [PK_langs]
    PRIMARY KEY CLUSTERED ([lang_id] ASC);
GO

-- Creating primary key on [format_id] in table 'formats'
ALTER TABLE [dbo].[formats]
ADD CONSTRAINT [PK_formats]
    PRIMARY KEY CLUSTERED ([format_id] ASC);
GO

-- Creating primary key on [country_id] in table 'countries'
ALTER TABLE [dbo].[countries]
ADD CONSTRAINT [PK_countries]
    PRIMARY KEY CLUSTERED ([country_id] ASC);
GO

-- Creating primary key on [person_id] in table 'people'
ALTER TABLE [dbo].[people]
ADD CONSTRAINT [PK_people]
    PRIMARY KEY CLUSTERED ([person_id] ASC);
GO

-- Creating primary key on [person_id] in table 'people_clients'
ALTER TABLE [dbo].[people_clients]
ADD CONSTRAINT [PK_people_clients]
    PRIMARY KEY CLUSTERED ([person_id] ASC);
GO

-- Creating primary key on [person_id] in table 'people_directors'
ALTER TABLE [dbo].[people_directors]
ADD CONSTRAINT [PK_people_directors]
    PRIMARY KEY CLUSTERED ([person_id] ASC);
GO

-- Creating primary key on [person_id] in table 'people_actors'
ALTER TABLE [dbo].[people_actors]
ADD CONSTRAINT [PK_people_actors]
    PRIMARY KEY CLUSTERED ([person_id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [client_id] in table 'orders'
ALTER TABLE [dbo].[orders]
ADD CONSTRAINT [FK_clientsorders]
    FOREIGN KEY ([client_id])
    REFERENCES [dbo].[people_clients]
        ([person_id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_clientsorders'
CREATE INDEX [IX_FK_clientsorders]
ON [dbo].[orders]
    ([client_id]);
GO

-- Creating foreign key on [person_id] in table 'people_clients'
ALTER TABLE [dbo].[people_clients]
ADD CONSTRAINT [FK_clients_inherits_person]
    FOREIGN KEY ([person_id])
    REFERENCES [dbo].[people]
        ([person_id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [person_id] in table 'people_directors'
ALTER TABLE [dbo].[people_directors]
ADD CONSTRAINT [FK_directors_inherits_person]
    FOREIGN KEY ([person_id])
    REFERENCES [dbo].[people]
        ([person_id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [person_id] in table 'people_actors'
ALTER TABLE [dbo].[people_actors]
ADD CONSTRAINT [FK_actors_inherits_person]
    FOREIGN KEY ([person_id])
    REFERENCES [dbo].[people]
        ([person_id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------