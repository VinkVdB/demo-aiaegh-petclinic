export class HelloWorldService {
    constructor() {
        this.greetings = { // ISO 639-3 codes
            eng: 'Hello, World!',       // English
            fra: 'Bonjour, le monde!',  // French
            nld: 'Hallo, wereld!'       // Dutch
        };
        this.codes = Object.keys(this.greetings);
    }

    // Returns a random greeting and its language code
    getRandomGreeting() {
        const code = this.codes[Math.floor(Math.random() * this.codes.length)];
        return { code, greeting: this.greetings[code] };
    }

    // Returns the greeting for a specific ISO 639-3 code
    getGreetingByCode(code) {
        return this.greetings[code] || null;
    }
}
