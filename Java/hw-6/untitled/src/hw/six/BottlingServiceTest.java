package hw.six;

import hw.six.exceptions.NoSuchBottleException;
import hw.six.exceptions.NotEnoughFilledBottlesException;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import java.util.ArrayList;
import java.util.List;

import static org.junit.Assert.*;
import static org.junit.jupiter.api.Assertions.assertDoesNotThrow;

class BottlingServiceTest {
    private List<Bottle> emptyBottles;
    private List<Bottle> filledBottles;

    private BottlingService bottlingService;

    @BeforeEach
    void setup() {
        Bottle bottle1 = new Bottle(MaterialType.GLASS);
        Bottle bottle2 = new Bottle(MaterialType.GLASS);

        Bottle bottle3 = new Bottle(MaterialType.PLASTIC);
        Bottle bottle4 = new Bottle(MaterialType.PLASTIC);

        this.emptyBottles = new ArrayList<>();
        this.emptyBottles.add(bottle1);
        this.emptyBottles.add(bottle2);

        this.filledBottles = new ArrayList<>();
        this.filledBottles.add(bottle3);
        this.filledBottles.add(bottle4);

        this.bottlingService = new BottlingService(emptyBottles, filledBottles);
    }

    @Test
    void when_thereIsEmptyBottleOfGivenMaterialType_then_returnTrue() throws NoSuchBottleException {
        assertTrue(this.bottlingService.fillBottle(MaterialType.GLASS));
    }

    @Test
    void when_thereIsNotEmptyBottleOfGivenMaterialType_then_NoSuchBottleExceptionIsThrown() {
        assertThrows(NoSuchBottleException.class, () -> this.bottlingService.fillBottle(MaterialType.PLASTIC));
    }

    @Test
    void when_thereIsNotEmptyBottleOfGivenMaterialType_then_NoSuchBottleExceptionIsThrown_And_MessageIsPrinted() {
        Exception exception = assertThrows(NoSuchBottleException.class, () -> this.bottlingService.fillBottle(MaterialType.PLASTIC));
        String expectedMessage = "Bottle of this material type is not in stock!";
        String actualMessage = exception.getMessage();
        assertEquals(expectedMessage, actualMessage);
    }

    @Test
    void when_thereAreEnoughEmptyBottlesInEmptyBottlesList() {
        assertDoesNotThrow(() -> this.bottlingService.fillBottles(MaterialType.GLASS, 2));
        assertEquals(4,this.bottlingService.getFilledBottles().size());
    }

    @Test
    void when_thereAreNotEnoughEmptyBottles_then_NoSuchBottleExceptionIsThrown() {
        assertThrows(NoSuchBottleException.class, () -> this.bottlingService.fillBottles(MaterialType.GLASS, 3));
    }

    @Test
    void when_thereAreNotEnoughEmptyBottles_then_NoSuchBottleExceptionIsThrown_And_MessageIsPrinted() {
        Exception exception = assertThrows(NoSuchBottleException.class, () -> this.bottlingService.fillBottles(MaterialType.GLASS, 3));
        String expectedMessage = "Bottle of this material type is not in stock!";
        String actualMessage = exception.getMessage();
        assertEquals(expectedMessage, actualMessage);
    }

    @Test
    void when_thereIsABottleOfTheNeededMaterialType_then_returnTrue() {
        assertTrue(this.bottlingService.sellFilledBottle(MaterialType.PLASTIC));
    }

    @Test
    void when_thereAreEnoughFilledBottles_then_ExceptionIsNotThrown() {
        assertDoesNotThrow(() -> this.bottlingService.sellFilledBottles(MaterialType.PLASTIC, 2));
        assertEquals(0,this.bottlingService.getFilledBottles().size());
    }

    @Test
    void when_thereAreNotEnoughFilledBottles_then_NotEnoughFilledBottlesExceptionIsThrown() {
        assertThrows(NotEnoughFilledBottlesException.class, () -> this.bottlingService.sellFilledBottles(MaterialType.PLASTIC, 3));
    }

    @Test
    void when_thereAreNotEnoughFilledBottles_then_NotEnoughFilledBottlesExceptionIsThrown_And_MessageIsPrinted() {
        Exception exception = assertThrows(NotEnoughFilledBottlesException.class, () -> this.bottlingService.sellFilledBottles(MaterialType.PLASTIC, 3));
        String expectedMessage = "Not enough filled bottles!";
        String actualMessage = exception.getMessage();
        assertEquals(expectedMessage, actualMessage);
    }


    @Test
    void when_thereAreEmptyBottlesOfGivenMaterialType_then_ReturnTrue() {
        assertTrue(this.bottlingService.hasBottleByTypeInEmptyBottlesList(MaterialType.GLASS));
    }

    @Test
    void when_thereAreNoEmptyBottlesOfGivenMaterialType_then_ReturnFalse() {
        assertFalse(this.bottlingService.hasBottleByTypeInEmptyBottlesList(MaterialType.PLASTIC));
    }

    @Test
    void whenThereAreTwoEmptyBottlesByMaterialType_thenReturnTwo() {
        int expected = 2;
        int actual = this.bottlingService.quantityOfEmptyBottlesByMaterialType(MaterialType.GLASS);
        assertEquals(expected, actual);
    }

    @Test
    void whenThereAreNoEmptyBottlesByMaterialType_thenReturnZero() {
        int expected = 0;
        int actual = this.bottlingService.quantityOfEmptyBottlesByMaterialType(MaterialType.PLASTIC);
        assertEquals(expected, actual);
    }


    @Test
    void whenThereAreTwoFilledBottlesByMaterialType_thenReturnTwo() {
        int expected = 2;
        int actual = this.bottlingService.quantityOfFilledBottlesByMaterialType(MaterialType.PLASTIC);
        assertEquals(expected, actual);
    }

    @Test
    void whenThereAreNoFilledBottlesByMaterialType_thenReturnZero() {
        int expected = 0;
        int actual = this.bottlingService.quantityOfFilledBottlesByMaterialType(MaterialType.GLASS);
        assertEquals(expected, actual);
    }

    @Test
    void whenFilledBottlesAreEnough_ThenReturnTrue() {
        assertTrue(this.bottlingService.hasEnoughFilledBottles(MaterialType.PLASTIC, 2));
    }

    @Test
    void whenFilledBottlesAreNotEnough_ThenReturnFalse() {
        assertFalse(this.bottlingService.hasEnoughFilledBottles(MaterialType.PLASTIC, 3));
    }
}