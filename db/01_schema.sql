USE TiendaDb;
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- CLIENTES
IF OBJECT_ID('dbo.Clientes','U') IS NULL
BEGIN
    CREATE TABLE dbo.Clientes(
        Id            INT IDENTITY(1,1) CONSTRAINT PK_Clientes PRIMARY KEY,
        Nombre        NVARCHAR(100)  NOT NULL,
        Apellidos     NVARCHAR(150)  NOT NULL,
        Direccion     NVARCHAR(250)  NULL,
        Email         NVARCHAR(150)  NOT NULL CONSTRAINT UQ_Clientes_Email UNIQUE,
        PasswordHash  NVARCHAR(200)  NOT NULL,
        FechaRegistro DATETIME2(3)   NOT NULL CONSTRAINT DF_Clientes_Fecha DEFAULT SYSUTCDATETIME(),
        CONSTRAINT CK_Clientes_Email CHECK (Email LIKE '%@%.%')
    );
END
GO

-- TIENDAS
IF OBJECT_ID('dbo.Tiendas','U') IS NULL
BEGIN
    CREATE TABLE dbo.Tiendas(
        Id        INT IDENTITY(1,1) CONSTRAINT PK_Tiendas PRIMARY KEY,
        Sucursal  NVARCHAR(120) NOT NULL,
        Direccion NVARCHAR(250) NULL
    );
END
GO

-- ARTICULOS
IF OBJECT_ID('dbo.Articulos','U') IS NULL
BEGIN
    CREATE TABLE dbo.Articulos(
        Id          INT IDENTITY(1,1) CONSTRAINT PK_Articulos PRIMARY KEY,
        Codigo      NVARCHAR(50)  NOT NULL CONSTRAINT UQ_Articulos_Codigo UNIQUE,
        Descripcion NVARCHAR(200) NOT NULL,
        Precio      DECIMAL(12,2) NOT NULL CONSTRAINT CK_Articulos_Precio CHECK (Precio >= 0),
        Imagen      NVARCHAR(512) NULL
    );
END
GO

-- ARTICULO x TIENDA (N:M + stock)
IF OBJECT_ID('dbo.ArticuloTienda','U') IS NULL
BEGIN
    CREATE TABLE dbo.ArticuloTienda(
        ArticuloId INT NOT NULL,
        TiendaId   INT NOT NULL,
        Stock      INT NOT NULL CONSTRAINT CK_ArticuloTienda_Stock CHECK (Stock >= 0),
        Fecha      DATETIME2(3) NOT NULL CONSTRAINT DF_ArticuloTienda_Fecha DEFAULT SYSUTCDATETIME(),
        CONSTRAINT PK_ArticuloTienda PRIMARY KEY (ArticuloId, TiendaId),
        CONSTRAINT FK_AT_Articulo FOREIGN KEY (ArticuloId) REFERENCES dbo.Articulos(Id),
        CONSTRAINT FK_AT_Tienda   FOREIGN KEY (TiendaId)   REFERENCES dbo.Tiendas(Id)
    );
END
GO

-- CLIENTE x ARTICULO (compras)
IF OBJECT_ID('dbo.ClienteArticulo','U') IS NULL
BEGIN
    CREATE TABLE dbo.ClienteArticulo(
        ClienteId INT NOT NULL,
        ArticuloId INT NOT NULL,
        Fecha     DATETIME2(3) NOT NULL CONSTRAINT DF_ClienteArticulo_Fecha DEFAULT SYSUTCDATETIME(),
        Cantidad  INT NOT NULL CONSTRAINT CK_ClienteArticulo_Cantidad CHECK (Cantidad > 0),
        CONSTRAINT PK_ClienteArticulo PRIMARY KEY (ClienteId, ArticuloId, Fecha),
        CONSTRAINT FK_CA_Cliente  FOREIGN KEY (ClienteId)  REFERENCES dbo.Clientes(Id),
        CONSTRAINT FK_CA_Articulo FOREIGN KEY (ArticuloId) REFERENCES dbo.Articulos(Id)
    );
END
GO

-- Carrito (uno activo por cliente)
IF OBJECT_ID('dbo.Carritos','U') IS NULL
BEGIN
    CREATE TABLE dbo.Carritos(
        Id         INT IDENTITY(1,1) CONSTRAINT PK_Carritos PRIMARY KEY,
        ClienteId  INT NOT NULL CONSTRAINT UQ_Carritos_Cliente UNIQUE,
        FechaCreacion DATETIME2(3) NOT NULL CONSTRAINT DF_Carritos_Fecha DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Carritos_Cliente FOREIGN KEY (ClienteId) REFERENCES dbo.Clientes(Id)
    );
END
GO

-- Items del carrito (por tienda y artículo)
IF OBJECT_ID('dbo.CarritoItems','U') IS NULL
BEGIN
    CREATE TABLE dbo.CarritoItems(
        Id         INT IDENTITY(1,1) CONSTRAINT PK_CarritoItems PRIMARY KEY,
        CarritoId  INT NOT NULL,
        ArticuloId INT NOT NULL,
        TiendaId   INT NOT NULL,
        Cantidad   INT NOT NULL CONSTRAINT CK_CarritoItems_Cantidad CHECK (Cantidad > 0),
        Fecha      DATETIME2(3) NOT NULL CONSTRAINT DF_CarritoItems_Fecha DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_CI_Carrito  FOREIGN KEY (CarritoId)  REFERENCES dbo.Carritos(Id),
        CONSTRAINT FK_CI_Articulo FOREIGN KEY (ArticuloId) REFERENCES dbo.Articulos(Id),
        CONSTRAINT FK_CI_Tienda   FOREIGN KEY (TiendaId)   REFERENCES dbo.Tiendas(Id),
        CONSTRAINT UQ_CI UNIQUE (CarritoId, ArticuloId, TiendaId)
    );
END
GO

IF OBJECT_ID('dbo.vwCatalogoPorTienda','V') IS NOT NULL
    DROP VIEW dbo.vwCatalogoPorTienda;
GO
CREATE VIEW dbo.vwCatalogoPorTienda AS
SELECT
    t.Id           AS TiendaId,
    t.Sucursal,
    a.Id           AS ArticuloId,
    a.Codigo,
    a.Descripcion,
    a.Precio,
    at.Stock,
    a.Imagen
FROM dbo.ArticuloTienda at
JOIN dbo.Tiendas t   ON t.Id = at.TiendaId
JOIN dbo.Articulos a ON a.Id = at.ArticuloId;
GO