-- Datos basicos de ejemplo para Azure SQL (Portafolio) - proyecto Farmacia
-- Correr DESPUES de PIV_PF_ProyectoFinal_Azure.sql

INSERT INTO TiposProducto (CodigoTipo, Descripcion) VALUES
('T001', 'Analgesicos'),
('T002', 'Antibioticos'),
('T003', 'Vitaminas'),
('T004', 'Higiene Personal');

INSERT INTO Productos (CodigoProducto, Descripcion, Precio, Estado, CodigoTipo, Cantidad) VALUES
('P001', 'Acetaminofen 500mg', 1200.00, 'En existencia', 'T001', 150),
('P002', 'Ibuprofeno 400mg', 1500.00, 'En existencia', 'T001', 120),
('P003', 'Amoxicilina 500mg', 3200.00, 'En existencia', 'T002', 80),
('P004', 'Vitamina C 1000mg', 2500.00, 'En existencia', 'T003', 200),
('P005', 'Jabon Antibacterial', 1800.00, 'En existencia', 'T004', 100);

INSERT INTO Clientes (Identificacion, NombreCompleto, Correo) VALUES
('101110111', 'Maria Rodriguez Solano', 'maria.rodriguez@correo.com'),
('202220222', 'Carlos Jimenez Vargas', 'carlos.jimenez@correo.com');

-- Cuentas de prueba, una por rol (contrasenas hasheadas, ver credenciales aparte)
INSERT INTO Usuarios_Farmacia (Identificacion, NombreCompleto, Correo, TipoUsuario, Estado, Contrasena) VALUES
('admin1', 'Ana Rodriguez', 'admin1@farmacia.com', 'Administrador', 'Activo', '100000.y7wOJqGYe/WjmHvwflj5RQ==.6yttfouLtX0sTDRmCufT2AYm0s3PzyqpzJJCgL6O+B4='),
('vend1', 'Luis Vargas', 'vend1@farmacia.com', 'Vendedor', 'Activo', '100000.ce2sb96JwW6V1Lfa1CJ9ag==.qFj+tyQSSd/aWqPHC5Q/fPduShu7cOS/9IA10Ub7OF0='),
('surt1', 'Karla Mendez', 'surt1@farmacia.com', 'Surtidor', 'Activo', '100000.aLyqTvIF/hpi0bYp0A0sjw==.jrIto5yS8MZr84fsgQ+AU1nh082qOHyxw5cC/27oapY='),
('cont1', 'Diego Solano', 'cont1@farmacia.com', 'Contador', 'Activo', '100000.ETvqqAUu90HPi83f8chYog==.al7HcdqRsg3ywZRtpDhksqbK4rqXeF3LOvBcZwNi1fQ=');
