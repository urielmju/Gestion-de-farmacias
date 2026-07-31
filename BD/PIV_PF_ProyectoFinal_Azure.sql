-- Estructura de la base de datos de Farmacia para Azure SQL (base compartida "Portafolio")
-- La tabla Usuarios se renombra a Usuarios_Farmacia para no chocar con la USUARIOS de Hospital.
-- El codigo C# no cambia: el mapeo de Entity Framework ya apunta a Usuarios_Farmacia.

CREATE TABLE TiposProducto (
    CodigoTipo VARCHAR(20) NOT NULL,
    Descripcion VARCHAR(200) NOT NULL,
    CONSTRAINT PK_TiposProducto PRIMARY KEY (CodigoTipo)
);

CREATE TABLE Productos (
    CodigoProducto VARCHAR(20) NOT NULL,
    Descripcion VARCHAR(200) NOT NULL,
    Precio DECIMAL(18,2) NOT NULL,
    Estado VARCHAR(15) NOT NULL,
    CodigoTipo VARCHAR(20) NOT NULL,
    Cantidad INT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Productos PRIMARY KEY (CodigoProducto),
    CONSTRAINT CK_Productos_Estado CHECK (Estado = 'En existencia' OR Estado = 'Agotado'),
    CONSTRAINT CK_Productos_Precio CHECK (Precio >= 0),
    CONSTRAINT FK_Productos_Tipo FOREIGN KEY (CodigoTipo) REFERENCES TiposProducto(CodigoTipo)
);

CREATE TABLE Clientes (
    IdCliente INT IDENTITY(1,1) NOT NULL,
    Identificacion VARCHAR(20) NOT NULL,
    NombreCompleto VARCHAR(150) NOT NULL,
    Correo VARCHAR(150) NOT NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_Clientes PRIMARY KEY (IdCliente),
    CONSTRAINT UQ_Clientes_Ident UNIQUE (Identificacion)
);

CREATE TABLE Usuarios_Farmacia (
    IdUsuario INT IDENTITY(1,1) NOT NULL,
    Identificacion VARCHAR(20) NOT NULL,
    NombreCompleto VARCHAR(150) NOT NULL,
    Correo VARCHAR(150) NOT NULL,
    TipoUsuario VARCHAR(20) NOT NULL,
    Estado VARCHAR(10) NOT NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    Contrasena VARCHAR(100) NULL,
    CONSTRAINT PK_Usuarios_Farmacia PRIMARY KEY (IdUsuario),
    CONSTRAINT UQ_UsuariosFarmacia_Ident UNIQUE (Identificacion),
    CONSTRAINT CK_UsuariosFarmacia_Estado CHECK (Estado = 'Inactivo' OR Estado = 'Activo'),
    CONSTRAINT CK_UsuariosFarmacia_Tipo CHECK (TipoUsuario IN ('Contador', 'Surtidor', 'Vendedor', 'Administrador'))
);

CREATE TABLE Facturas (
    CodigoFactura VARCHAR(20) NOT NULL,
    Fecha DATETIME NOT NULL,
    IdCliente INT NOT NULL,
    MetodoPago VARCHAR(10) NOT NULL,
    Subtotal DECIMAL(18,2) NOT NULL,
    Recargo DECIMAL(18,2) NOT NULL DEFAULT 0,
    Total DECIMAL(18,2) NOT NULL,
    CONSTRAINT PK_Facturas PRIMARY KEY (CodigoFactura),
    CONSTRAINT CK_Facturas_Metodo CHECK (MetodoPago = 'Tarjeta' OR MetodoPago = 'Efectivo'),
    CONSTRAINT CK_Facturas_Subtotal CHECK (Subtotal >= 0),
    CONSTRAINT CK_Facturas_Total CHECK (Total >= 0),
    CONSTRAINT FK_Facturas_Cliente FOREIGN KEY (IdCliente) REFERENCES Clientes(IdCliente)
);

CREATE TABLE DetalleFactura (
    IdDetalle INT IDENTITY(1,1) NOT NULL,
    CodigoFactura VARCHAR(20) NOT NULL,
    CodigoProducto VARCHAR(20) NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(18,2) NOT NULL,
    SubtotalLinea DECIMAL(18,2) NOT NULL,
    CONSTRAINT PK_DetalleFactura PRIMARY KEY (IdDetalle),
    CONSTRAINT CK_Detalle_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT CK_Detalle_Precio CHECK (PrecioUnitario >= 0),
    CONSTRAINT FK_Detalle_Producto FOREIGN KEY (CodigoProducto) REFERENCES Productos(CodigoProducto),
    CONSTRAINT FK_Detalle_Factura FOREIGN KEY (CodigoFactura) REFERENCES Facturas(CodigoFactura)
);
