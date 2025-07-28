class Point {
  #x;
  #y;
  constructor(x, y) {
    this.#x = x;
    this.#y = y;
  }

  getX() {
    return this.#x;
  }

  getY() {
    return this.#y;
  }

  distance(other) {
    return Math.sqrt(Math.pow(this.#x - other.getX(), 2) + Math.pow(this.#y - other.getY(), 2));
  }
}

// Example usage
const point1 = new Point(3, 4);
const point2 = new Point(6, 8);
console.log(`Distance between point1 and point2: ${point1.distance(point2)}`);