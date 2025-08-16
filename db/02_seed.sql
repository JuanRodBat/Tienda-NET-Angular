-- Clientes (PasswordHash)
INSERT INTO dbo.Clientes (Nombre, Apellidos, Direccion, Email, PasswordHash)
VALUES ('Juan', 'Pérez', 'Av. Siempre Viva 123', 'juan@example.com', 'to_set_in_api');

-- Tiendas
INSERT INTO dbo.Tiendas (Sucursal, Direccion)
VALUES ('Centro', 'Calle 1'), ('Norte', 'Calle 2');

-- Artículos
INSERT INTO dbo.Articulos (Codigo, Descripcion, Precio, Imagen)
VALUES ('A001','Café molido 500g',120.00,NULL),
       ('A002','Taza cerámica 300ml',80.00,NULL),
       ('A003','Galletas artesanales',55.00,NULL);

-- Stock por tienda
INSERT INTO dbo.ArticuloTienda (ArticuloId, TiendaId, Stock)
SELECT a.Id, t.Id, v.Stock
FROM (VALUES ('A001', 20), ('A002', 50), ('A003', 35)) v(Codigo, Stock)
JOIN dbo.Articulos a ON a.Codigo = v.Codigo
CROSS JOIN (SELECT Id FROM dbo.Tiendas) t;  -- stock igual en todas las tiendas

-- Compra de ejemplo
DECLARE @clienteId INT = (SELECT TOP 1 Id FROM dbo.Clientes ORDER BY Id);
DECLARE @artA001 INT  = (SELECT Id FROM dbo.Articulos WHERE Codigo='A001');
INSERT INTO dbo.ClienteArticulo (ClienteId, ArticuloId, Cantidad) VALUES (@clienteId, @artA001, 2);

-- Búsqueda por Tienda → Artículos
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AT_Tienda' AND object_id = OBJECT_ID('dbo.ArticuloTienda'))
    CREATE NONCLUSTERED INDEX IX_AT_Tienda ON dbo.ArticuloTienda (TiendaId) INCLUDE (ArticuloId, Stock);

-- Búsqueda por Articulo en stock
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AT_Articulo' AND object_id = OBJECT_ID('dbo.ArticuloTienda'))
    CREATE NONCLUSTERED INDEX IX_AT_Articulo ON dbo.ArticuloTienda (ArticuloId) INCLUDE (TiendaId, Stock);

-- Historial de compras por cliente
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_CA_Cliente_Fecha' AND object_id = OBJECT_ID('dbo.ClienteArticulo'))
    CREATE NONCLUSTERED INDEX IX_CA_Cliente_Fecha ON dbo.ClienteArticulo (ClienteId, Fecha DESC) INCLUDE (ArticuloId, Cantidad);