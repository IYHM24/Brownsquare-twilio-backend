import { UserSeed } from './user.seed';
import typeormConfig from '../../config/typeorm.config';

async function runSeeds() {
  const dataSource = await typeormConfig.initialize();

  try {
    const userSeed = new UserSeed();
    await userSeed.run(dataSource);

    console.log('✅ All seeds completed successfully!');
  } catch (error) {
    console.error('❌ Error running seeds:', error);
    process.exit(1);
  } finally {
    await dataSource.destroy();
  }
}

runSeeds();
