
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/21/2022 20:17:35
-- Generated from EDMX file: E:\university\WPF\movies.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [DB];
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
IF OBJECT_ID(N'[dbo].[FK_movieslangs]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[languages] DROP CONSTRAINT [FK_movieslangs];
GO
IF OBJECT_ID(N'[dbo].[FK_moviesgenres]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[genres] DROP CONSTRAINT [FK_moviesgenres];
GO
IF OBJECT_ID(N'[dbo].[FK_moviesorders]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[orders] DROP CONSTRAINT [FK_moviesorders];
GO
IF OBJECT_ID(N'[dbo].[FK_moviescountries]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[countries] DROP CONSTRAINT [FK_moviescountries];
GO
IF OBJECT_ID(N'[dbo].[FK_moviesroles]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[roles] DROP CONSTRAINT [FK_moviesroles];
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
IF OBJECT_ID(N'[dbo].[languages]', 'U') IS NOT NULL
    DROP TABLE [dbo].[languages];
GO
IF OBJECT_ID(N'[dbo].[countries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[countries];
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
    [order_id] int  NOT NULL
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

-- Creating table 'actors'
CREATE TABLE [dbo].[actors] (
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
    [client_id] int  NOT NULL
);
GO

-- Creating table 'genres'
CREATE TABLE [dbo].[genres] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(100)  NOT NULL,
    [movie_id] int  NOT NULL
);
GO

-- Creating table 'languages'
CREATE TABLE [dbo].[languages] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(100)  NOT NULL,
    [movie_id] int  NOT NULL
);
GO

-- Creating table 'countries'
CREATE TABLE [dbo].[countries] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(200)  NOT NULL,
    [movie_id] int  NOT NULL
);
GO

-- Creating table 'formats'
CREATE TABLE [dbo].[formats] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL,
    [movie_id] int  NOT NULL
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

-- Creating primary key on [Id] in table 'actors'
ALTER TABLE [dbo].[actors]
ADD CONSTRAINT [PK_actors]
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

-- Creating primary key on [id] in table 'languages'
ALTER TABLE [dbo].[languages]
ADD CONSTRAINT [PK_languages]
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

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [actor_Id] in table 'roles'
ALTER TABLE [dbo].[roles]
ADD CONSTRAINT [FK_authorwork]
    FOREIGN KEY ([actor_Id])
    REFERENCES [dbo].[actors]
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

-- Creating foreign key on [movie_id] in table 'languages'
ALTER TABLE [dbo].[languages]
ADD CONSTRAINT [FK_movieslangs]
    FOREIGN KEY ([movie_id])
    REFERENCES [dbo].[movies]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_movieslangs'
CREATE INDEX [IX_FK_movieslangs]
ON [dbo].[languages]
    ([movie_id]);
GO

-- Creating foreign key on [movie_id] in table 'genres'
ALTER TABLE [dbo].[genres]
ADD CONSTRAINT [FK_moviesgenres]
    FOREIGN KEY ([movie_id])
    REFERENCES [dbo].[movies]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_moviesgenres'
CREATE INDEX [IX_FK_moviesgenres]
ON [dbo].[genres]
    ([movie_id]);
GO

-- Creating foreign key on [movie_id] in table 'countries'
ALTER TABLE [dbo].[countries]
ADD CONSTRAINT [FK_moviescountries]
    FOREIGN KEY ([movie_id])
    REFERENCES [dbo].[movies]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_moviescountries'
CREATE INDEX [IX_FK_moviescountries]
ON [dbo].[countries]
    ([movie_id]);
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

-- Creating foreign key on [movie_id] in table 'formats'
ALTER TABLE [dbo].[formats]
ADD CONSTRAINT [FK_moviesformats]
    FOREIGN KEY ([movie_id])
    REFERENCES [dbo].[movies]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_moviesformats'
CREATE INDEX [IX_FK_moviesformats]
ON [dbo].[formats]
    ([movie_id]);
GO

-- Creating foreign key on [order_id] in table 'movies'
ALTER TABLE [dbo].[movies]
ADD CONSTRAINT [FK_ordersmovies]
    FOREIGN KEY ([order_id])
    REFERENCES [dbo].[orders]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ordersmovies'
CREATE INDEX [IX_FK_ordersmovies]
ON [dbo].[movies]
    ([order_id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------