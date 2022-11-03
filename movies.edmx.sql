
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/03/2022 17:35:08
-- Generated from EDMX file: E:\university\wypozyczalnia WPF\WPF\movies.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [E:\UNIVERSITY\WYPOZYCZALNIA WPF\WPF\DB.MDF];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'movies'
CREATE TABLE [dbo].[movies] (
    [movie_id] int IDENTITY(1,1) NOT NULL,
    [title] nvarchar(200)  NOT NULL,
    [year] nvarchar(4)  NOT NULL,
    [duration] tinyint  NOT NULL,
    [price] decimal(18,0)  NOT NULL,
    [description] nvarchar(500)  NOT NULL,
    [min_age] tinyint  NOT NULL,
    [publisher_Id] int  NOT NULL
);
GO

-- Creating table 'clients'
CREATE TABLE [dbo].[clients] (
    [client_id] int IDENTITY(1,1) NOT NULL,
    [phone] nvarchar(12)  NOT NULL,
    [email] nvarchar(250)  NULL,
    [first_name] nvarchar(100)  NOT NULL,
    [last_name] nvarchar(100)  NOT NULL
);
GO

-- Creating table 'authors'
CREATE TABLE [dbo].[authors] (
    [author_Id] int IDENTITY(1,1) NOT NULL,
    [first_name] nvarchar(100)  NOT NULL,
    [last_name] nvarchar(100)  NOT NULL,
    [movies_movie_id] int  NOT NULL
);
GO

-- Creating table 'roles'
CREATE TABLE [dbo].[roles] (
    [work_Id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(150)  NOT NULL,
    [author_author_Id] int  NOT NULL
);
GO

-- Creating table 'orders'
CREATE TABLE [dbo].[orders] (
    [order_id] int IDENTITY(1,1) NOT NULL,
    [quantity] tinyint  NOT NULL,
    [draw_date] nvarchar(max)  NOT NULL,
    [due_date] datetime  NOT NULL,
    [client_client_id] int  NOT NULL
);
GO

-- Creating table 'genres'
CREATE TABLE [dbo].[genres] (
    [genre_id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(100)  NOT NULL,
    [movies_movie_id] int  NOT NULL
);
GO

-- Creating table 'copies'
CREATE TABLE [dbo].[copies] (
    [copy_id] int IDENTITY(1,1) NOT NULL,
    [movies_movie_id] int  NOT NULL,
    [order_order_id] int  NOT NULL
);
GO

-- Creating table 'publishers'
CREATE TABLE [dbo].[publishers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(150)  NOT NULL
);
GO

-- Creating table 'langs'
CREATE TABLE [dbo].[langs] (
    [lang_id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(100)  NOT NULL,
    [country_country_id] int  NOT NULL,
    [movies_movie_id] int  NOT NULL
);
GO

-- Creating table 'countries'
CREATE TABLE [dbo].[countries] (
    [country_id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(200)  NOT NULL
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

-- Creating primary key on [client_id] in table 'clients'
ALTER TABLE [dbo].[clients]
ADD CONSTRAINT [PK_clients]
    PRIMARY KEY CLUSTERED ([client_id] ASC);
GO

-- Creating primary key on [author_Id] in table 'authors'
ALTER TABLE [dbo].[authors]
ADD CONSTRAINT [PK_authors]
    PRIMARY KEY CLUSTERED ([author_Id] ASC);
GO

-- Creating primary key on [work_Id] in table 'roles'
ALTER TABLE [dbo].[roles]
ADD CONSTRAINT [PK_roles]
    PRIMARY KEY CLUSTERED ([work_Id] ASC);
GO

-- Creating primary key on [order_id] in table 'orders'
ALTER TABLE [dbo].[orders]
ADD CONSTRAINT [PK_orders]
    PRIMARY KEY CLUSTERED ([order_id] ASC);
GO

-- Creating primary key on [genre_id] in table 'genres'
ALTER TABLE [dbo].[genres]
ADD CONSTRAINT [PK_genres]
    PRIMARY KEY CLUSTERED ([genre_id] ASC);
GO

-- Creating primary key on [copy_id] in table 'copies'
ALTER TABLE [dbo].[copies]
ADD CONSTRAINT [PK_copies]
    PRIMARY KEY CLUSTERED ([copy_id] ASC);
GO

-- Creating primary key on [Id] in table 'publishers'
ALTER TABLE [dbo].[publishers]
ADD CONSTRAINT [PK_publishers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [lang_id] in table 'langs'
ALTER TABLE [dbo].[langs]
ADD CONSTRAINT [PK_langs]
    PRIMARY KEY CLUSTERED ([lang_id] ASC);
GO

-- Creating primary key on [country_id] in table 'countries'
ALTER TABLE [dbo].[countries]
ADD CONSTRAINT [PK_countries]
    PRIMARY KEY CLUSTERED ([country_id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [movies_movie_id] in table 'copies'
ALTER TABLE [dbo].[copies]
ADD CONSTRAINT [FK_moviesproduct]
    FOREIGN KEY ([movies_movie_id])
    REFERENCES [dbo].[movies]
        ([movie_id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_moviesproduct'
CREATE INDEX [IX_FK_moviesproduct]
ON [dbo].[copies]
    ([movies_movie_id]);
GO

-- Creating foreign key on [author_author_Id] in table 'roles'
ALTER TABLE [dbo].[roles]
ADD CONSTRAINT [FK_authorwork]
    FOREIGN KEY ([author_author_Id])
    REFERENCES [dbo].[authors]
        ([author_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_authorwork'
CREATE INDEX [IX_FK_authorwork]
ON [dbo].[roles]
    ([author_author_Id]);
GO

-- Creating foreign key on [movies_movie_id] in table 'authors'
ALTER TABLE [dbo].[authors]
ADD CONSTRAINT [FK_moviesauthors]
    FOREIGN KEY ([movies_movie_id])
    REFERENCES [dbo].[movies]
        ([movie_id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_moviesauthors'
CREATE INDEX [IX_FK_moviesauthors]
ON [dbo].[authors]
    ([movies_movie_id]);
GO

-- Creating foreign key on [client_client_id] in table 'orders'
ALTER TABLE [dbo].[orders]
ADD CONSTRAINT [FK_clientsorders]
    FOREIGN KEY ([client_client_id])
    REFERENCES [dbo].[clients]
        ([client_id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_clientsorders'
CREATE INDEX [IX_FK_clientsorders]
ON [dbo].[orders]
    ([client_client_id]);
GO

-- Creating foreign key on [publisher_Id] in table 'movies'
ALTER TABLE [dbo].[movies]
ADD CONSTRAINT [FK_publishermovies]
    FOREIGN KEY ([publisher_Id])
    REFERENCES [dbo].[publishers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_publishermovies'
CREATE INDEX [IX_FK_publishermovies]
ON [dbo].[movies]
    ([publisher_Id]);
GO

-- Creating foreign key on [order_order_id] in table 'copies'
ALTER TABLE [dbo].[copies]
ADD CONSTRAINT [FK_orderscopies]
    FOREIGN KEY ([order_order_id])
    REFERENCES [dbo].[orders]
        ([order_id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_orderscopies'
CREATE INDEX [IX_FK_orderscopies]
ON [dbo].[copies]
    ([order_order_id]);
GO

-- Creating foreign key on [country_country_id] in table 'langs'
ALTER TABLE [dbo].[langs]
ADD CONSTRAINT [FK_countrieslangs]
    FOREIGN KEY ([country_country_id])
    REFERENCES [dbo].[countries]
        ([country_id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_countrieslangs'
CREATE INDEX [IX_FK_countrieslangs]
ON [dbo].[langs]
    ([country_country_id]);
GO

-- Creating foreign key on [movies_movie_id] in table 'langs'
ALTER TABLE [dbo].[langs]
ADD CONSTRAINT [FK_movieslangs]
    FOREIGN KEY ([movies_movie_id])
    REFERENCES [dbo].[movies]
        ([movie_id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_movieslangs'
CREATE INDEX [IX_FK_movieslangs]
ON [dbo].[langs]
    ([movies_movie_id]);
GO

-- Creating foreign key on [movies_movie_id] in table 'genres'
ALTER TABLE [dbo].[genres]
ADD CONSTRAINT [FK_moviesgenres]
    FOREIGN KEY ([movies_movie_id])
    REFERENCES [dbo].[movies]
        ([movie_id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_moviesgenres'
CREATE INDEX [IX_FK_moviesgenres]
ON [dbo].[genres]
    ([movies_movie_id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------