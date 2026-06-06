
CREATE DATABASE PIV_PF_ProyectoFinal;
GO

USE PIV_PF_ProyectoFinal;
GO
CREATE TABLE Usuarios
(
    IdUsuario        INT           IDENTITY(1,1) NOT NULL,
    Identificacion   VARCHAR(20)   NOT NULL,
    NombreCompleto   VARCHAR(150)  NOT NULL,
    Correo           VARCHAR(150)  NOT NULL,
    Contrasena       VARCHAR(100)  NOT NULL,
    TipoUsuario      VARCHAR(20)   NOT NULL,
    Estado           VARCHAR(10)   NOT NULL,
    FechaRegistro    DATETIME      NOT NULL CONSTRAINT DF_Usuarios_FechaRegistro DEFAULT (GETDATE()),

    CONSTRAINT PK_Usuarios        PRIMARY KEY (IdUsuario),
    CONSTRAINT UQ_Usuarios_Ident  UNIQUE (Identificacion),
    CONSTRAINT CK_Usuarios_Tipo   CHECK (TipoUsuario IN ('Administrador','Vendedor','Surtidor','Contador')),
    CONSTRAINT CK_Usuarios_Estado CHECK (Estado IN ('Activo','Inactivo'))
);
GO
CREATE TABLE Clientes
(
    IdCliente        INT           IDENTITY(1,1) NOT NULL,
    Identificacion   VARCHAR(20)   NOT NULL,
    NombreCompleto   VARCHAR(150)  NOT NULL,
    Correo           VARCHAR(150)  NOT NULL,
    FechaRegistro    DATETIME      NOT NULL CONSTRAINT DF_Clientes_FechaRegistro DEFAULT (GETDATE()),

    CONSTRAINT PK_Clientes       PRIMARY KEY (IdCliente),
    CONSTRAINT UQ_Clientes_Ident UNIQUE (Identificacion)
);
GO
CREATE TABLE TiposProducto
(
    CodigoTipo  VARCHAR(20)  NOT NULL,
    Descripcion VARCHAR(200) NOT NULL,

    CONSTRAINT PK_TiposProducto PRIMARY KEY (CodigoTipo)
);
GO
CREATE TABLE Productos
(
    CodigoProducto VARCHAR(20)    NOT NULL,
    Descripcion    VARCHAR(200)   NOT NULL,
    Precio         DECIMAL(18,2)  NOT NULL,
    Cantidad       INT            NOT NULL CONSTRAINT DF_Productos_Cantidad DEFAULT 0,
    Estado         VARCHAR(15)    NOT NULL,
    CodigoTipo     VARCHAR(20)    NOT NULL,

    CONSTRAINT PK_Productos        PRIMARY KEY (CodigoProducto),
    CONSTRAINT FK_Productos_Tipo   FOREIGN KEY (CodigoTipo) REFERENCES TiposProducto(CodigoTipo),
    CONSTRAINT CK_Productos_Estado CHECK (Estado IN ('Agotado','En existencia')),
    CONSTRAINT CK_Productos_Precio CHECK (Precio >= 0),
    CONSTRAINT CK_Productos_Cant   CHECK (Cantidad >= 0)
);
GO
CREATE TABLE Facturas
(
    CodigoFactura VARCHAR(20)    NOT NULL,
    Fecha         DATETIME       NOT NULL,
    IdCliente     INT            NOT NULL,
    MetodoPago    VARCHAR(10)    NOT NULL,
    Subtotal      DECIMAL(18,2)  NOT NULL,
    Recargo       DECIMAL(18,2)  NOT NULL CONSTRAINT DF_Facturas_Recargo DEFAULT (0),
    Total         DECIMAL(18,2)  NOT NULL,

    CONSTRAINT PK_Facturas          PRIMARY KEY (CodigoFactura),
    CONSTRAINT FK_Facturas_Cliente  FOREIGN KEY (IdCliente) REFERENCES Clientes(IdCliente),
    CONSTRAINT CK_Facturas_Metodo   CHECK (MetodoPago IN ('Efectivo','Tarjeta')),
    CONSTRAINT CK_Facturas_Subtotal CHECK (Subtotal >= 0),
    CONSTRAINT CK_Facturas_Total    CHECK (Total >= 0)
);
GO
CREATE TABLE DetalleFactura
(
    IdDetalle      INT            IDENTITY(1,1) NOT NULL,
    CodigoFactura  VARCHAR(20)    NOT NULL,
    CodigoProducto VARCHAR(20)    NOT NULL,
    Cantidad       INT            NOT NULL,
    PrecioUnitario DECIMAL(18,2)  NOT NULL,
    SubtotalLinea  DECIMAL(18,2)  NOT NULL,

    CONSTRAINT PK_DetalleFactura   PRIMARY KEY (IdDetalle),
    CONSTRAINT FK_Detalle_Factura  FOREIGN KEY (CodigoFactura)
                                   REFERENCES Facturas(CodigoFactura)
                                   ON DELETE CASCADE,
    CONSTRAINT FK_Detalle_Producto FOREIGN KEY (CodigoProducto)
                                   REFERENCES Productos(CodigoProducto),
    CONSTRAINT CK_Detalle_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT CK_Detalle_Precio   CHECK (PrecioUnitario >= 0)
);
GO
-- usuario administrador 
INSERT INTO Usuarios (Identificacion, NombreCompleto, Correo, Contrasena, TipoUsuario, Estado)
VALUES ('101010101', 'Administrador del Sistema', 'admin@farmacia.com', 'Admin@1', 'Administrador', 'Activo');

-- tipos de producto
INSERT INTO TiposProducto (CodigoTipo, Descripcion) VALUES
('TP001', 'Analgesicos'),
('TP002', 'Antibioticos'),
('TP003', 'Vitaminas y suplementos'),
('TP004', 'Cuidado personal');

-- productos
INSERT INTO Productos (CodigoProducto, Descripcion, Precio, Cantidad, Estado, CodigoTipo) VALUES
('P001', 'Acetaminofen 500mg - 10 tabletas',  1500.00, 50, 'En existencia', 'TP001'),
('P002', 'Ibuprofeno 400mg - 20 tabletas',    2200.00, 30, 'En existencia', 'TP001'),
('P003', 'Amoxicilina 500mg - 12 capsulas',   4500.00, 20, 'En existencia', 'TP002'),
('P004', 'Vitamina C 1000mg - 30 tabletas',   3800.00, 40, 'En existencia', 'TP003'),
('P005', 'Jabon antibacterial 120g',            900.00,  0, 'Agotado',       'TP004');

-- cliente
INSERT INTO Clientes (Identificacion, NombreCompleto, Correo) VALUES
('202020202', 'Juan Perez Rodriguez',  'juan.perez@correo.com'),
('303030303', 'Maria Gonzalez Lopez',  'maria.gonzalez@correo.com');
GO

---auxiliares
CREATE INDEX IX_Productos_Tipo    ON Productos(CodigoTipo);
CREATE INDEX IX_Facturas_Cliente  ON Facturas(IdCliente);
CREATE INDEX IX_Facturas_Fecha    ON Facturas(Fecha);
CREATE INDEX IX_Detalle_Factura   ON DetalleFactura(CodigoFactura);
CREATE INDEX IX_Detalle_Producto  ON DetalleFactura(CodigoProducto);
GO

--pruebas 
SELECT 'Usuarios'       AS Tabla, COUNT(*) AS Registros FROM Usuarios
UNION ALL SELECT 'Clientes',       COUNT(*) FROM Clientes
UNION ALL SELECT 'TiposProducto',  COUNT(*) FROM TiposProducto
UNION ALL SELECT 'Productos',      COUNT(*) FROM Productos
UNION ALL SELECT 'Facturas',       COUNT(*) FROM Facturas
UNION ALL SELECT 'DetalleFactura', COUNT(*) FROM DetalleFactura;
GO