import { DataSource } from 'typeorm';
import { User } from '../../modules/users/entities/user.entity';
import * as bcrypt from 'bcrypt';

export class UserSeed {
  public async run(dataSource: DataSource): Promise<void> {
    const repository = dataSource.getRepository(User);

    // Check if admin user already exists
    const existingAdmin = await repository.findOne({
      where: { email: 'admin@example.com' },
    });

    if (!existingAdmin) {
      const adminUser = repository.create({
        email: 'admin@example.com',
        firstName: 'Admin',
        lastName: 'User',
        password: await bcrypt.hash('Admin123!', 12),
        role: 'admin',
        isActive: true,
      });

      await repository.save(adminUser);
      console.log('Admin user created successfully');
    }

    // Create a regular user
    const existingUser = await repository.findOne({
      where: { email: 'user@example.com' },
    });

    if (!existingUser) {
      const regularUser = repository.create({
        email: 'user@example.com',
        firstName: 'Regular',
        lastName: 'User',
        password: await bcrypt.hash('User123!', 12),
        role: 'user',
        isActive: true,
      });

      await repository.save(regularUser);
      console.log('Regular user created successfully');
    }
  }
}
