# 🚀 NestJS Professional Backend Template

Una plantilla profesional y escalable para backend usando NestJS con arquitectura limpia, diseñada siguiendo las mejores prácticas y principios SOLID.

## 📋 Tabla de Contenidos

- [Características](#características)
- [Arquitectura](#arquitectura)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Instalación](#instalación)
- [Configuración](#configuración)
- [Uso](#uso)
- [API Documentation](#api-documentation)
- [Testing](#testing)
- [Docker](#docker)
- [Deployment](#deployment)
- [Contribución](#contribución)

## ✨ Características

### 🏗️ Arquitectura y Diseño
- **Clean Architecture** con separación clara de responsabilidades
- **Domain-Driven Design (DDD)** para modelado de dominio
- **Principios SOLID** aplicados en todo el código
- **Dependency Injection** nativo de NestJS
- **TypeScript** con configuración estricta

### 🔐 Autenticación y Autorización
- **JWT Authentication** con Passport
- **Role-based Access Control (RBAC)**
- **Guards personalizados** para protección de rutas
- **Rate limiting** con Throttler

### 📊 Base de Datos
- **TypeORM** con soporte para PostgreSQL
- **Migrations** automáticas
- **Seeds** para datos iniciales
- **Connection pooling** configurado

### 🛡️ Seguridad
- **Helmet** para headers de seguridad
- **CORS** configurado
- **Validation pipes** con class-validator
- **Global exception filter**

### 📈 Observabilidad
- **Winston** para logging estructurado
- **Health checks** detallados
- **Request/Response interceptors**
- **Global exception handling**

### 🧪 Testing
- **Unit tests** con Jest
- **Integration tests** (E2E)
- **Coverage reports**
- **Test utilities** personalizadas

### 📦 DevOps
- **Docker** multi-stage builds
- **Docker Compose** para desarrollo
- **GitHub Actions** (opcional)
- **Environment-based configuration**

## 🏛️ Arquitectura

Esta plantilla implementa una **Arquitectura Limpia** (Clean Architecture) con las siguientes capas:

```
┌─────────────────────────────────────┐
│           Controllers               │  ← Presentation Layer
│         (HTTP Interface)            │
├─────────────────────────────────────┤
│             Services                │  ← Application Layer  
│        (Business Logic)             │
├─────────────────────────────────────┤
│           Entities                  │  ← Domain Layer
│       (Domain Models)               │
├─────────────────────────────────────┤
│           Repository                │  ← Infrastructure Layer
│      (Data Access)                  │
└─────────────────────────────────────┘
```

### Principios Aplicados

- **Separation of Concerns**: Cada módulo tiene una responsabilidad específica
- **Dependency Inversion**: Las capas superiores no dependen de las inferiores
- **Single Responsibility**: Cada clase tiene una única razón para cambiar
- **Open/Closed**: Abierto para extensión, cerrado para modificación

## 📁 Estructura del Proyecto

```
src/
├── common/                     # Utilidades compartidas
│   ├── filters/               # Exception filters
│   ├── interceptors/          # Request/Response interceptors
│   └── guards/               # Guards personalizados
├── config/                   # Configuraciones
│   └── database.config.ts    # Configuración de base de datos
├── modules/                  # Módulos de negocio
│   ├── auth/                # Autenticación y autorización
│   │   ├── decorators/      # Decoradores personalizados
│   │   ├── dto/            # Data Transfer Objects
│   │   ├── guards/         # Guards de autenticación
│   │   ├── strategies/     # Estrategias de Passport
│   │   ├── auth.controller.ts
│   │   ├── auth.service.ts
│   │   └── auth.module.ts
│   ├── users/              # Gestión de usuarios
│   │   ├── dto/           # DTOs para usuarios
│   │   ├── entities/      # Entidades de dominio
│   │   ├── users.controller.ts
│   │   ├── users.service.ts
│   │   └── users.module.ts
│   └── health/            # Health checks
├── app.controller.ts       # Controlador principal
├── app.service.ts         # Servicio principal
├── app.module.ts          # Módulo raíz
└── main.ts               # Punto de entrada
```

## 🚀 Instalación

### Prerequisitos

- Node.js (v18 o superior)
- PostgreSQL (v12 o superior)
- npm o yarn

### Pasos de instalación

1. **Clonar el repositorio**
```bash
git clone <your-repo-url>
cd twilio-service
```

2. **Instalar dependencias**
```bash
npm install
```

3. **Configurar variables de entorno**
```bash
cp .env.example .env
```

4. **Configurar base de datos**
```bash
# Crear base de datos PostgreSQL
createdb nestjs_template
```

5. **Ejecutar migraciones**
```bash
npm run migration:run
```

6. **Inicializar datos (opcional)**
```bash
npm run seed
```

## ⚙️ Configuración

### Variables de Entorno

Crear un archivo `.env` basado en `.env.example`:

```env
# Application
NODE_ENV=development
PORT=3000

# Database
DATABASE_HOST=localhost
DATABASE_PORT=5432
DATABASE_USERNAME=postgres
DATABASE_PASSWORD=password
DATABASE_NAME=nestjs_template

# JWT
JWT_SECRET=your-super-secret-jwt-key
JWT_EXPIRES_IN=24h

# CORS
CORS_ORIGIN=http://localhost:3000
```

### Configuración de Base de Datos

La configuración de TypeORM se encuentra en `src/config/database.config.ts` y soporta:

- **Connection pooling**
- **SSL en producción**
- **Logging en desarrollo**
- **Migraciones automáticas**

## 🎮 Uso

### Desarrollo

```bash
# Modo desarrollo con hot-reload
npm run start:dev

# Modo debug
npm run start:debug
```

### Producción

```bash
# Compilar
npm run build

# Ejecutar en producción
npm run start:prod
```

### Comandos Útiles

```bash
# Linting
npm run lint

# Formateo de código
npm run format

# Testing
npm run test
npm run test:watch
npm run test:cov

# E2E testing
npm run test:e2e
```

## 📚 API Documentation

### Swagger UI

La documentación interactiva de la API está disponible en:
- **Desarrollo**: http://localhost:3000/api/docs
- **Producción**: https://your-domain.com/api/docs

### Endpoints Principales

#### Autenticación
```
POST /api/v1/auth/login      - Iniciar sesión
POST /api/v1/auth/register   - Registrar usuario
```

#### Usuarios
```
GET    /api/v1/users         - Listar usuarios (Admin)
GET    /api/v1/users/:id     - Obtener usuario
POST   /api/v1/users         - Crear usuario (Admin)
PATCH  /api/v1/users/:id     - Actualizar usuario
DELETE /api/v1/users/:id     - Eliminar usuario (Admin)
PATCH  /api/v1/users/:id/deactivate - Desactivar usuario (Admin)
```

#### Health Checks
```
GET /api/v1/health           - Estado básico
GET /api/v1/health/detailed  - Estado detallado
```

### Autenticación

La API utiliza **JWT Bearer Token**:

```bash
# Ejemplo de request con autenticación
curl -H "Authorization: Bearer <your-jwt-token>" \
     http://localhost:3000/api/v1/users
```

## 🧪 Testing

### Estructura de Tests

```
test/
├── jest-e2e.json           # Configuración E2E
└── app.e2e-spec.ts        # Tests de integración

src/
└── **/*.spec.ts           # Unit tests
```

### Ejecutar Tests

```bash
# Unit tests
npm run test

# Tests con coverage
npm run test:cov

# E2E tests
npm run test:e2e

# Tests en modo watch
npm run test:watch
```

### Ejemplo de Test

```typescript
describe('UsersService', () => {
  let service: UsersService;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      providers: [UsersService],
    }).compile();

    service = module.get<UsersService>(UsersService);
  });

  it('should create a user', async () => {
    const userDto = { email: 'test@test.com', firstName: 'John', lastName: 'Doe' };
    const result = await service.create(userDto);
    expect(result).toBeDefined();
  });
});
```

## 🐳 Docker

### Desarrollo Local

```bash
# Construir imagen
docker build -t nestjs-backend .

# Ejecutar con Docker Compose
docker-compose up -d
```

### Producción

```bash
# Build optimizado para producción
docker build --target production -t nestjs-backend:prod .

# Ejecutar en producción
docker run -d -p 3000:3000 \
  -e NODE_ENV=production \
  -e DATABASE_HOST=your-db-host \
  nestjs-backend:prod
```

### Docker Compose

El archivo `docker-compose.yml` incluye:
- **Aplicación NestJS**
- **PostgreSQL database**
- **Volúmenes persistentes**
- **Network configuration**

## 🚀 Deployment

### Preparación para Producción

1. **Variables de entorno**
```bash
export NODE_ENV=production
export JWT_SECRET=your-production-secret
export DATABASE_URL=postgresql://user:pass@host:5432/db
```

2. **Build de producción**
```bash
npm run build
```

3. **Migraciones**
```bash
npm run migration:run
```

### Plataformas Recomendadas

- **Heroku**: Deploy directo desde Git
- **AWS ECS/Fargate**: Containerized deployment
- **DigitalOcean App Platform**: Managed deployment
- **Vercel**: Para APIs pequeñas
- **Railway**: Deployment simplificado

### Health Checks

La aplicación incluye endpoints de health check para monitoreo:

```bash
# Health check básico
curl http://localhost:3000/api/v1/health

# Health check detallado
curl http://localhost:3000/api/v1/health/detailed
```

## 📊 Monitoring y Logging

### Winston Logger

Configurado con múltiples transports:
- **Console**: Para desarrollo
- **File**: Para logs persistentes
- **JSON format**: Para análisis estructurado

### Request Logging

Todos los requests son logueados con:
- Método HTTP
- URL
- Tiempo de respuesta
- IP del cliente

### Error Handling

Sistema robusto de manejo de errores:
- **Global Exception Filter**
- **Structured error responses**
- **Stack traces en desarrollo**

## 🔧 Extensibilidad

### Agregar Nuevos Módulos

1. **Generar módulo**
```bash
nest generate module products
nest generate controller products
nest generate service products
```

2. **Crear entidad**
```typescript
@Entity('products')
export class Product {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Column()
  name: string;
}
```

3. **Configurar módulo**
```typescript
@Module({
  imports: [TypeOrmModule.forFeature([Product])],
  controllers: [ProductsController],
  providers: [ProductsService],
})
export class ProductsModule {}
```

### Middleware Personalizado

```typescript
@Injectable()
export class CustomMiddleware implements NestMiddleware {
  use(req: Request, res: Response, next: NextFunction) {
    // Tu lógica aquí
    next();
  }
}
```

### Guards Personalizados

```typescript
@Injectable()
export class CustomGuard implements CanActivate {
  canActivate(context: ExecutionContext): boolean {
    // Tu lógica de autorización
    return true;
  }
}
```

## 🤝 Contribución

### Proceso de Desarrollo

1. **Fork** el repositorio
2. **Create** una branch para tu feature
3. **Commit** tus cambios
4. **Push** a la branch
5. **Create** un Pull Request

### Convenciones de Código

- **ESLint + Prettier** para formato
- **Conventional Commits** para mensajes
- **Tests obligatorios** para nuevas features
- **Documentation updates** cuando sea necesario

### Git Hooks

```bash
# Instalar husky para git hooks
npm install --save-dev husky
npx husky install

# Pre-commit hook
npx husky add .husky/pre-commit "npm run lint && npm run test"
```

## 📝 Licencia

Este proyecto está bajo la licencia [MIT](LICENSE).

## 🙋‍♂️ Soporte

Si tienes preguntas o necesitas ayuda:

1. **Issues**: Para reportar bugs o solicitar features
2. **Discussions**: Para preguntas generales
3. **Wiki**: Para documentación detallada

## 🎯 Roadmap

- [ ] GraphQL support
- [ ] Redis caching
- [ ] File upload service
- [ ] Email service
- [ ] Background jobs with Bull
- [ ] API versioning
- [ ] Rate limiting per user
- [ ] Audit logging

---

**¡Gracias por usar esta plantilla! 🙏**

Si te resulta útil, no olvides darle una ⭐ al repositorio.