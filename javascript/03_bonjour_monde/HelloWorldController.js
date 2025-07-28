export class HelloWorldController {
    constructor(service) {
        this.service = service;
    }

    // Display a random greeting
    showRandomGreeting() {
        const { code, greeting } = this.service.getRandomGreeting();
        console.log(`[${code}] ${greeting}`);
    }

    // Display a greeting for a specific code
    showGreetingByCode(code) {
        const greeting = this.service.getGreetingByCode(code);
        if (greeting) {
            console.log(`[${code}] ${greeting}`);
        } else {
            console.log(`No greeting found for code: ${code}`);
        }
    }
}
