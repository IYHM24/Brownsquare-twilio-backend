import { Injectable } from '@nestjs/common';

@Injectable()
export class AppService {
  getAppInfo() {
    return {
      name: 'NestJS Professional Backend',
      version: '1.0.0',
      description: 'Professional NestJS Backend with Clean Architecture',
      timestamp: new Date().toISOString(),
    };
  }
}
