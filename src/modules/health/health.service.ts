import { Injectable } from '@nestjs/common';
import { InjectDataSource } from '@nestjs/typeorm';
import { DataSource } from 'typeorm';

@Injectable()
export class HealthService {
  constructor(
    @InjectDataSource()
    private readonly dataSource: DataSource,
  ) {}

  checkHealth() {
    return {
      status: 'ok',
      timestamp: new Date().toISOString(),
      uptime: process.uptime(),
      environment: process.env.NODE_ENV || 'development',
    };
  }

  async detailedHealth() {
    const basicHealth = this.checkHealth();

    try {
      // Check database connection
      await this.dataSource.query('SELECT 1');
      const dbStatus = 'connected';

      return {
        ...basicHealth,
        services: {
          database: {
            status: dbStatus,
            type: this.dataSource.options.type,
          },
        },
        memory: process.memoryUsage(),
        version: process.version,
      };
    } catch (error: any) {
      return {
        ...basicHealth,
        services: {
          database: {
            status: 'error',
            error: error?.message || 'Unknown error',
          },
        },
        memory: process.memoryUsage(),
        version: process.version,
      };
    }
  }
}
