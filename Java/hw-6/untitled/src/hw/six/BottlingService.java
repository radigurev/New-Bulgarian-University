package hw.six;

import hw.six.exceptions.NoSuchBottleException;
import hw.six.exceptions.NotEnoughFilledBottlesException;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

public class BottlingService {
    private final List<Bottle> emptyBottles;
    private final List<Bottle> filledBottles;

    public BottlingService() {
        this.emptyBottles = new ArrayList<>();
        this.filledBottles = new ArrayList<>();
    }

    public BottlingService(List<Bottle> emptyBottles,
                           List<Bottle> filledBottles) {
        this.emptyBottles = emptyBottles;
        this.filledBottles = filledBottles;
    }

    public List<Bottle> getEmptyBottles() {
        return emptyBottles;
    }

    public List<Bottle> getFilledBottles() {
        return filledBottles;
    }

    public boolean fillBottle(MaterialType materialType)
            throws NoSuchBottleException {

        Iterator<Bottle> it = emptyBottles.iterator();
        while (it.hasNext()) {
            Bottle b = it.next();
            if (b.getMaterialType() == materialType) {
                it.remove();
                filledBottles.add(b);
                return true;
            }
        }
        throw new NoSuchBottleException(
                "Bottle of this material type is not in stock!");
    }

    public void fillBottles(MaterialType materialType, int quantity)
            throws NoSuchBottleException {

        if (quantityOfEmptyBottlesByMaterialType(materialType) < quantity) {
            throw new NoSuchBottleException(
                    "Bottle of this material type is not in stock!");
        }

        int filled = 0;
        Iterator<Bottle> it = emptyBottles.iterator();
        while (it.hasNext() && filled < quantity) {
            Bottle b = it.next();
            if (b.getMaterialType() == materialType) {
                it.remove();
                filledBottles.add(b);
                filled++;
            }
        }
    }

    public boolean removeEmptyBottle(MaterialType materialType)
            throws NoSuchBottleException {

        Iterator<Bottle> it = emptyBottles.iterator();
        while (it.hasNext()) {
            Bottle b = it.next();
            if (b.getMaterialType() == materialType) {
                it.remove();
                return true;
            }
        }
        throw new NoSuchBottleException(
                "Bottle of this material type is not in stock!");
    }

    public boolean sellFilledBottle(MaterialType materialType)
            throws NotEnoughFilledBottlesException {

        Iterator<Bottle> it = filledBottles.iterator();
        while (it.hasNext()) {
            Bottle b = it.next();
            if (b.getMaterialType() == materialType) {
                it.remove();
                return true;
            }
        }
        throw new NotEnoughFilledBottlesException(
                "Not enough filled bottles!");
    }

    public void sellFilledBottles(MaterialType materialType, int quantity)
            throws NotEnoughFilledBottlesException {

        if (!hasEnoughFilledBottles(materialType, quantity)) {
            throw new NotEnoughFilledBottlesException(
                    "Not enough filled bottles!");
        }

        int sold = 0;
        Iterator<Bottle> it = filledBottles.iterator();
        while (it.hasNext() && sold < quantity) {
            Bottle b = it.next();
            if (b.getMaterialType() == materialType) {
                it.remove();
                sold++;
            }
        }
    }

    public boolean hasBottleByTypeInEmptyBottlesList(MaterialType type) {
        return emptyBottles.stream()
                .anyMatch(b -> b.getMaterialType() == type);
    }

    public int quantityOfEmptyBottlesByMaterialType(MaterialType type) {
        return (int) emptyBottles.stream()
                .filter(b -> b.getMaterialType() == type)
                .count();
    }

    public int quantityOfFilledBottlesByMaterialType(MaterialType type) {
        return (int) filledBottles.stream()
                .filter(b -> b.getMaterialType() == type)
                .count();
    }

    public boolean hasEnoughFilledBottles(MaterialType type, int qty) {
        return quantityOfFilledBottlesByMaterialType(type) >= qty;
    }
}
