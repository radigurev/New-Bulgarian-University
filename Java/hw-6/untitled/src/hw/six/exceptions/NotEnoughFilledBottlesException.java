package hw.six.exceptions;

public class NotEnoughFilledBottlesException extends RuntimeException {
    public NotEnoughFilledBottlesException(String message) {
        super(message);
    }
}