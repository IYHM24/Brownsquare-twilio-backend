import { Controller, Get } from '@nestjs/common';
import { ApiTags, ApiOperation, ApiResponse } from '@nestjs/swagger';
import { HealthService } from './health.service';

@ApiTags('Health')
@Controller('health')
export class HealthController {
  constructor(private readonly healthService: HealthService) {}

  @Get()
  @ApiOperation({ summary: 'Health check' })
  @ApiResponse({ status: 200, description: 'Service is healthy' })
  checkHealth() {
    return this.healthService.checkHealth();
  }

  @Get('detailed')
  @ApiOperation({ summary: 'Detailed health check' })
  @ApiResponse({ status: 200, description: 'Detailed health information' })
  detailedHealth() {
    return this.healthService.detailedHealth();
  }
}
