# webProject2


this is a book store program.
you can either be an admin or a customer

here are the tables you need to use the project


CREATE TABLE [dbo].[cart] (
    [Id]       INT          IDENTITY (1, 1) NOT NULL,
    [name]     VARCHAR (50) NULL,
    [quantity] INT          NULL,
    [userid]   INT          NULL,
    [itemid]   INT          NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[items] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [name]        VARCHAR (50)  NULL,
    [description] VARCHAR (MAX) NULL,
    [price]       INT           NULL,
    [quantity]    INT           NULL,
    [discount]    VARCHAR (50)  NULL,
    [category]    VARCHAR (50)  NULL,
    [image]       VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[orders] (
    [Id]       INT      IDENTITY (1, 1) NOT NULL,
    [userid]   INT      NULL,
    [itemid]   INT      NULL,
    [buyDate]  DATETIME NULL,
    [quantity] INT      NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_orders_items] FOREIGN KEY ([itemid]) REFERENCES [dbo].[items] ([Id])
);

CREATE TABLE [dbo].[roles] (
    [Id]     INT          IDENTITY (1, 1) NOT NULL,
    [FKRole] VARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[userid] (
    [Id]     INT IDENTITY (1, 1) NOT NULL,
    [userid] INT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_userid_users] FOREIGN KEY ([userid]) REFERENCES [dbo].[users] ([Id])
);



CREATE TABLE [dbo].[users] (
    [Id]           INT          IDENTITY (1, 1) NOT NULL,
    [name]         VARCHAR (50) NULL,
    [password]     VARCHAR (50) NULL,
    [role]         VARCHAR (50) NULL,
    [registerDate] DATETIME     NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);












