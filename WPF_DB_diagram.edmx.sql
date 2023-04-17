
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/17/2023 18:15:50
-- Generated from EDMX file: E:\university\WPF\WPF_DB_diagram.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [E:\UNIVERSITY\WPF\WPF_DB.MDF];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_moviescountries]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[movies] DROP CONSTRAINT [FK_moviescountries];
GO
IF OBJECT_ID(N'[dbo].[FK_movieslangs]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[movies] DROP CONSTRAINT [FK_movieslangs];
GO
IF OBJECT_ID(N'[dbo].[FK_moviesformats]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[movies] DROP CONSTRAINT [FK_moviesformats];
GO
IF OBJECT_ID(N'[dbo].[FK_moviesdirectors]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[movies] DROP CONSTRAINT [FK_moviesdirectors];
GO
IF OBJECT_ID(N'[dbo].[FK_moviesactors]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[movies] DROP CONSTRAINT [FK_moviesactors];
GO
IF OBJECT_ID(N'[dbo].[FK_clientsorders]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[orders] DROP CONSTRAINT [FK_clientsorders];
GO
IF OBJECT_ID(N'[dbo].[FK_moviesorders]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[orders] DROP CONSTRAINT [FK_moviesorders];
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
IF OBJECT_ID(N'[dbo].[directors]', 'U') IS NOT NULL
    DROP TABLE [dbo].[directors];
GO
IF OBJECT_ID(N'[dbo].[actors]', 'U') IS NOT NULL
    DROP TABLE [dbo].[actors];
GO
IF OBJECT_ID(N'[dbo].[clients]', 'U') IS NOT NULL
    DROP TABLE [dbo].[clients];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'movies'
CREATE TABLE [dbo].[movies] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL,
    [year] smallint  NULL,
    [country_id] int  NULL,
    [duration] smallint  NULL,
    [age] tinyint  NULL,
    [total_count] tinyint  NULL,
    [price] decimal(18,0)  NULL,
    [left_count] tinyint  NULL,
    [plot] nvarchar(max)  NULL,
    [lang_id] int  NULL,
    [actor_id] int  NULL,
    [director_id] int  NULL,
    [format_id] int  NULL,
    [poster_path] nvarchar(max)  NULL,
    [trailer_path] nvarchar(max)  NULL,
    [genre_id] int  NULL
);
GO

-- Creating table 'orders'
CREATE TABLE [dbo].[orders] (
    [id] int IDENTITY(1,1) NOT NULL,
    [client_id] int  NOT NULL,
    [movie_id] int  NOT NULL,
    [rent_date] datetime  NOT NULL,
    [due_date] datetime  NULL,
    [return_date] datetime  NULL
);
GO

-- Creating table 'langs'
CREATE TABLE [dbo].[langs] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'formats'
CREATE TABLE [dbo].[formats] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'countries'
CREATE TABLE [dbo].[countries] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'directors'
CREATE TABLE [dbo].[directors] (
    [id] int IDENTITY(1,1) NOT NULL,
    [first_name] nvarchar(max)  NOT NULL,
    [last_name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'actors'
CREATE TABLE [dbo].[actors] (
    [id] int IDENTITY(1,1) NOT NULL,
    [first_name] nvarchar(max)  NOT NULL,
    [last_name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'clients'
CREATE TABLE [dbo].[clients] (
    [id] int IDENTITY(1,1) NOT NULL,
    [phone] nvarchar(max)  NULL,
    [email] nvarchar(max)  NULL,
    [first_name] nvarchar(max)  NOT NULL,
    [last_name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'genres'
CREATE TABLE [dbo].[genres] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL
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

-- Creating primary key on [id] in table 'orders'
ALTER TABLE [dbo].[orders]
ADD CONSTRAINT [PK_orders]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'langs'
ALTER TABLE [dbo].[langs]
ADD CONSTRAINT [PK_langs]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'formats'
ALTER TABLE [dbo].[formats]
ADD CONSTRAINT [PK_formats]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'countries'
ALTER TABLE [dbo].[countries]
ADD CONSTRAINT [PK_countries]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'directors'
ALTER TABLE [dbo].[directors]
ADD CONSTRAINT [PK_directors]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'actors'
ALTER TABLE [dbo].[actors]
ADD CONSTRAINT [PK_actors]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'clients'
ALTER TABLE [dbo].[clients]
ADD CONSTRAINT [PK_clients]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'genres'
ALTER TABLE [dbo].[genres]
ADD CONSTRAINT [PK_genres]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [country_id] in table 'movies'
ALTER TABLE [dbo].[movies]
ADD CONSTRAINT [FK_moviescountries]
    FOREIGN KEY ([country_id])
    REFERENCES [dbo].[countries]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_moviescountries'
CREATE INDEX [IX_FK_moviescountries]
ON [dbo].[movies]
    ([country_id]);
GO

-- Creating foreign key on [lang_id] in table 'movies'
ALTER TABLE [dbo].[movies]
ADD CONSTRAINT [FK_movieslangs]
    FOREIGN KEY ([lang_id])
    REFERENCES [dbo].[langs]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_movieslangs'
CREATE INDEX [IX_FK_movieslangs]
ON [dbo].[movies]
    ([lang_id]);
GO

-- Creating foreign key on [format_id] in table 'movies'
ALTER TABLE [dbo].[movies]
ADD CONSTRAINT [FK_moviesformats]
    FOREIGN KEY ([format_id])
    REFERENCES [dbo].[formats]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_moviesformats'
CREATE INDEX [IX_FK_moviesformats]
ON [dbo].[movies]
    ([format_id]);
GO

-- Creating foreign key on [director_id] in table 'movies'
ALTER TABLE [dbo].[movies]
ADD CONSTRAINT [FK_moviesdirectors]
    FOREIGN KEY ([director_id])
    REFERENCES [dbo].[directors]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_moviesdirectors'
CREATE INDEX [IX_FK_moviesdirectors]
ON [dbo].[movies]
    ([director_id]);
GO

-- Creating foreign key on [actor_id] in table 'movies'
ALTER TABLE [dbo].[movies]
ADD CONSTRAINT [FK_moviesactors]
    FOREIGN KEY ([actor_id])
    REFERENCES [dbo].[actors]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_moviesactors'
CREATE INDEX [IX_FK_moviesactors]
ON [dbo].[movies]
    ([actor_id]);
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

-- Creating foreign key on [movie_id] in table 'orders'
ALTER TABLE [dbo].[orders]
ADD CONSTRAINT [FK_moviesorders]
    FOREIGN KEY ([movie_id])
    REFERENCES [dbo].[movies]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_moviesorders'
CREATE INDEX [IX_FK_moviesorders]
ON [dbo].[orders]
    ([movie_id]);
GO

-- Creating foreign key on [genre_id] in table 'movies'
ALTER TABLE [dbo].[movies]
ADD CONSTRAINT [FK_genresmovies]
    FOREIGN KEY ([genre_id])
    REFERENCES [dbo].[genres]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_genresmovies'
CREATE INDEX [IX_FK_genresmovies]
ON [dbo].[movies]
    ([genre_id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------