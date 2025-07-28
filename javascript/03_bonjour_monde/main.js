import { HelloWorldService } from './HelloWorldService.js';
import { HelloWorldController } from './HelloWorldController.js';

const service = new HelloWorldService();
const controller = new HelloWorldController(service);

controller.showRandomGreeting(); // Random greeting
controller.showGreetingByCode('fra'); // French greeting
controller.showGreetingByCode('nld'); // Dutch greeting
controller.showGreetingByCode('eng'); // English greeting
controller.showGreetingByCode('spa'); // Not found
