# DoubleV Partners - Sistema de Gestión de Facturas

## 📋 Descripción General

Sistema completo de gestión de facturas desarrollado con arquitectura moderna que incluye una API REST en .NET Core 9.0 y un frontend en Angular 18. El sistema permite la gestión completa de clientes, productos y facturas con funcionalidades avanzadas como validación en tiempo real, paginación del servidor y búsqueda inteligente.

## 🚀 Características Principales

### Backend (API)
- ✅ **API REST** completa con .NET Core 9.0
- ✅ **Base de Datos** SQL Server con procedimientos almacenados
- ✅ **Paginación** automática en todos los endpoints
- ✅ **Validación** de datos y manejo de errores
- ✅ **Documentación** automática con Swagger
- ✅ **CORS** configurado para desarrollo frontend
- ✅ **Librería Personalizada** LibraryDBApi para acceso a datos

### Frontend (Angular)
- ✅ **Interfaz Moderna** con Angular 18 y Bootstrap 5
- ✅ **Formularios Reactivos** con validación en tiempo real
- ✅ **Búsqueda Inteligente** por número de factura o cliente
- ✅ **Validación Asíncrona** del número de factura
- ✅ **Paginación del Servidor** con filtros persistentes
- ✅ **Estados de Carga** y feedback visual
- ✅ **Responsive Design** para móviles y desktop

## 🛠️ Tecnologías Utilizadas

### Backend
- **.NET Core 9.0** - Framework principal
- **SQL Server** - Base de datos
- **🚀 LibraryDBApi 1.1.1** - **Librería personalizada desarrollada por Daniel Martin** para acceso a datos de alto rendimiento
- **Swagger/OpenAPI** - Documentación automática

### Frontend
- **Angular 18** - Framework principal
- **TypeScript** - Lenguaje de programación
- **Bootstrap 5** - Framework CSS

## 🚀 **LIBRERÍA PERSONALIZADA - LibraryDBApi**

### ⭐ **Desarrollada por Daniel Martin**

Este proyecto utiliza **LibraryDBApi 1.1.1**, una librería NuGet **desarrollada específicamente por [Daniel Martin](https://www.nuget.org/packages/LibraryDBApi)** que revoluciona el acceso a datos en .NET:

### 📦 **Disponible en NuGet:**
```bash
dotnet add package LibraryDBApi --version 1.1.1
```

### 🌟 **¿Por qué LibraryDBApi?**
- **Desarrollada por un experto** en .NET y SQL Server
- **Optimizada para rendimiento** en aplicaciones empresariales
- **Documentación completa** y soporte activo
- **Código abierto** y mantenido activamente
- **Compatible** con .NET Core 6.0+ y .NET Framework 4.8+

## 🚀 Instalación y Configuración

### Prerrequisitos
- .NET Core 9.0 SDK
- Node.js 18+ y npm
- SQL Server (Local o remoto)
- Visual Studio 2022 o VS Code

### 1. Clonar el Repositorio
```bash
git clone [URL_DEL_REPOSITORIO]
cd DoubleV-Partners
```

### 3. Configurar la API
```bash
cd DoubleVPartnersApi
dotnet restore
dotnet run
```

### 4. Configurar el Frontend
```bash
cd DoubleVPartnersFront
npm install
ng serve
```

## 📚 Endpoints de la API

### Clientes
- `GET /api/Clientes/get` - Lista paginada de clientes
- `GET /api/Clientes/get/{id}` - Cliente por ID
- `POST /api/Clientes/create` - Crear cliente
- `PUT /api/Clientes/update` - Actualizar cliente
- `DELETE /api/Clientes/delete/{id}` - Eliminar cliente

### Productos
- `GET /api/Productos/get` - Lista paginada de productos
- `GET /api/Productos/get/{id}` - Producto por ID
- `POST /api/Productos/create` - Crear producto
- `PUT /api/Productos/update` - Actualizar producto
- `DELETE /api/Productos/delete/{id}` - Eliminar producto

### Facturas
- `GET /api/Facturas/get` - Lista paginada de facturas
- `GET /api/Facturas/GetByNumeroFactura/{numero}` - Buscar por número
- `GET /api/Facturas/GetByClienteId/{clienteId}` - Facturas por cliente
- `GET /api/Facturas/GetDetallesByIdFactura/{id}` - Detalles de factura
- `POST /api/Facturas/create` - Crear factura

## 🎯 Funcionalidades del Sistema

### Gestión de Facturas
- **Creación** con validación de número único
- **Búsqueda** por número o cliente
- **Listado** con paginación y filtros
- **Cálculo Automático** de totales e impuestos
- **Gestión de Detalles** con productos dinámicos

### Validación en Tiempo Real
- **Verificación Asíncrona** del número de factura
- **Feedback Visual** inmediato
- **Prevención de Duplicados** automática

## 👨‍💻 **DESARROLLADOR PRINCIPAL**

### 🏆 **Daniel Martin** - Desarrollador Full Stack & Creador de LibraryDBApi

**Daniel Martin** es el desarrollador principal de este proyecto y el **creador de la librería LibraryDBApi**, una solución innovadora para el acceso a datos en .NET.

### 🌟 **Logros Destacados:**
- ✅ **Creador de LibraryDBApi** - Librería NuGet publicada y disponible
- ✅ **Desarrollador Full Stack** con experiencia en .NET y Angular
- ✅ **Especialista en SQL Server** y optimización de consultas
- ✅ **Arquitecto de Software** con enfoque en rendimiento y escalabilidad

### 🔗 **Enlaces Importantes:**
- 📦 **NuGet Package:** [LibraryDBApi](https://www.nuget.org/packages/LibraryDBApi)
- 🐙 **GitHub:** [DANII3L](https://github.com/DANII3L)
