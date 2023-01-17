
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 01/17/2023 21:12:45
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

IF OBJECT_ID(N'[dbo].[FK_authorwork]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[roles] DROP CONSTRAINT [FK_authorwork];
GO
IF OBJECT_ID(N'[dbo].[FK_clientsorders]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[orders] DROP CONSTRAINT [FK_clientsorders];
GO
IF OBJECT_ID(N'[dbo].[FK_moviesgenres]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[genres] DROP CONSTRAINT [FK_moviesgenres];
GO
IF OBJECT_ID(N'[dbo].[FK_moviescountries]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[countries] DROP CONSTRAINT [FK_moviescountries];
GO
IF OBJECT_ID(N'[dbo].[FK_moviesroles]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[roles] DROP CONSTRAINT [FK_moviesroles];
GO
IF OBJECT_ID(N'[dbo].[FK_moviesformats]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[formats] DROP CONSTRAINT [FK_moviesformats];
GO
IF OBJECT_ID(N'[dbo].[FK_ordersmovies]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[movies] DROP CONSTRAINT [FK_ordersmovies];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[movies]', 'U') IS NOT NULL
    DROP TABLE [dbo].[movies];
GO
IF OBJECT_ID(N'[dbo].[clients]', 'U') IS NOT NULL
    DROP TABLE [dbo].[clients];
GO
IF OBJECT_ID(N'[dbo].[actors]', 'U') IS NOT NULL
    DROP TABLE [dbo].[actors];
GO
IF OBJECT_ID(N'[dbo].[roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[roles];
GO
IF OBJECT_ID(N'[dbo].[orders]', 'U') IS NOT NULL
    DROP TABLE [dbo].[orders];
GO
IF OBJECT_ID(N'[dbo].[genres]', 'U') IS NOT NULL
    DROP TABLE [dbo].[genres];
GO
IF OBJECT_ID(N'[dbo].[countries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[countries];
GO
IF OBJECT_ID(N'[dbo].[formats]', 'U') IS NOT NULL
    DROP TABLE [dbo].[formats];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'movies'
CREATE TABLE [dbo].[movies] (
    [id] int IDENTITY(1,1) NOT NULL,
    [title] nvarchar(200)  NOT NULL,
    [year] nvarchar(4)  NOT NULL,
    [duration] tinyint  NOT NULL,
    [price] decimal(18,0)  NOT NULL,
    [description] nvarchar(500)  NOT NULL,
    [age] tinyint  NOT NULL,
    [genre_id] nvarchar(max)  NOT NULL,
    [format_id] nvarchar(max)  NOT NULL,
    [country_id] nvarchar(max)  NOT NULL,
    [lead_role_id] nvarchar(max)  NOT NULL,
    [director_id] nvarchar(max)  NOT NULL,
    [ordersmovies_movies_id] int  NOT NULL
);
GO

-- Creating table 'clients'
CREATE TABLE [dbo].[clients] (
    [id] int IDENTITY(1,1) NOT NULL,
    [phone] nvarchar(12)  NOT NULL,
    [email] nvarchar(250)  NULL,
    [first_name] nvarchar(100)  NOT NULL,
    [last_name] nvarchar(100)  NOT NULL
);
GO

-- Creating table 'lead_roles'
CREATE TABLE [dbo].[lead_roles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [first_name] nvarchar(100)  NOT NULL,
    [last_name] nvarchar(100)  NOT NULL
);
GO

-- Creating table 'roles'
CREATE TABLE [dbo].[roles] (
    [id] int IDENTITY(1,1) NOT NULL,
    [actor_Id] int  NOT NULL,
    [movie_id] int  NOT NULL
);
GO

-- Creating table 'orders'
CREATE TABLE [dbo].[orders] (
    [id] int IDENTITY(1,1) NOT NULL,
    [quantity] tinyint  NOT NULL,
    [rent_date] datetime  NOT NULL,
    [due_date] datetime  NOT NULL,
    [return_date] datetime  NOT NULL,
    [movie_id] nvarchar(max)  NOT NULL,
    [client_id] int  NOT NULL
);
GO

-- Creating table 'genres'
CREATE TABLE [dbo].[genres] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(100)  NOT NULL
);
GO

-- Creating table 'countries'
CREATE TABLE [dbo].[countries] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(200)  NOT NULL
);
GO

-- Creating table 'formats'
CREATE TABLE [dbo].[formats] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'directors'
CREATE TABLE [dbo].[directors] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [first_name] nvarchar(max)  NOT NULL,
    [last_name] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [id] in table 'movies'
ALTER TABLE [dbo].[movies]
ADD CONSTRAINT [PK_movies]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'clients'
ALTER TABLE [dbo].[clients]
ADD CONSTRAINT [PK_clients]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [Id] in table 'lead_roles'
ALTER TABLE [dbo].[lead_roles]
ADD CONSTRAINT [PK_lead_roles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [id] in table 'roles'
ALTER TABLE [dbo].[roles]
ADD CONSTRAINT [PK_roles]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'orders'
ALTER TABLE [dbo].[orders]
ADD CONSTRAINT [PK_orders]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'genres'
ALTER TABLE [dbo].[genres]
ADD CONSTRAINT [PK_genres]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'countries'
ALTER TABLE [dbo].[countries]
ADD CONSTRAINT [PK_countries]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'formats'
ALTER TABLE [dbo].[formats]
ADD CONSTRAINT [PK_formats]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [Id] in table 'directors'
ALTER TABLE [dbo].[directors]
ADD CONSTRAINT [PK_directors]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [actor_Id] in table 'roles'
ALTER TABLE [dbo].[roles]
ADD CONSTRAINT [FK_authorwork]
    FOREIGN KEY ([actor_Id])
    REFERENCES [dbo].[lead_roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_authorwork'
CREATE INDEX [IX_FK_authorwork]
ON [dbo].[roles]
    ([actor_Id]);
GO

-- Creating foreign key on [client_id] in table 'orders'
ALTER TABLE [dbo].[orders]
ADD CONSTRAINT [FK_clientsorders]
    FOREIGN KEY ([client_id])
    REFERENCES [dbo].[clients]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_clientsorders'
CREATE INDEX [IX_FK_clientsorders]
ON [dbo].[orders]
    ([client_id]);
GO

-- Creating foreign key on [movie_id] in table 'roles'
ALTER TABLE [dbo].[roles]
ADD CONSTRAINT [FK_moviesroles]
    FOREIGN KEY ([movie_id])
    REFERENCES [dbo].[movies]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_moviesroles'
CREATE INDEX [IX_FK_moviesroles]
ON [dbo].[roles]
    ([movie_id]);
GO

-- Creating foreign key on [ordersmovies_movies_id] in table 'movies'
ALTER TABLE [dbo].[movies]
ADD CONSTRAINT [FK_ordersmovies]
    FOREIGN KEY ([ordersmovies_movies_id])
    REFERENCES [dbo].[orders]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ordersmovies'
CREATE INDEX [IX_FK_ordersmovies]
ON [dbo].[movies]
    ([ordersmovies_movies_id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------