package org.example;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;

interface Deliverable {
    double deliveryPrice();
}

abstract class DeliverableItem implements Deliverable {
    private double additionalPrice;
    private boolean deliverToClient;

    public DeliverableItem(boolean deliverToClient) {
        setDeliverToClient(deliverToClient);
    }

    @Override
    public double deliveryPrice() {
        return deliverToClient ? additionalPrice : 0;
    }

    public double getAdditionalPrice() {
        return additionalPrice;
    }

    public void setAdditionalPrice(double price) {
        this.additionalPrice = price;
    }

    public boolean isDeliverToClient() {
        return deliverToClient;
    }

    public void setDeliverToClient(boolean deliverToClient) {
        this.deliverToClient = deliverToClient;
    }
}

class Material {
    private final String name;
    private final boolean IsFragile;

    public Material(String name, boolean isFragile) {
        this.name = name;
        IsFragile = isFragile;
    }

    public String getName() {
        return name;
    }

    public boolean isFragile() {
        return IsFragile;
    }
}

class Document extends DeliverableItem {
    private final double minPrice;

    public Document(boolean deliverToClient, double minPrice) {
        super(deliverToClient);
        this.minPrice = minPrice;
    }

    public double getMinPrice() {
        return minPrice;
    }

    @Override
    public double deliveryPrice() {
        return getMinPrice() + super.deliveryPrice();
    }
}

class WeightedItem extends DeliverableItem {

    private final Material material;
    private final double weight;
    private final double pricePerKilo;

    public WeightedItem(boolean deliverToClient, Material material, double weight, double pricePerKilo) {
        super(deliverToClient);
        this.material = material;
        this.weight = weight;
        this.pricePerKilo = pricePerKilo;
    }

    public Material getMaterial() {
        return material;
    }

    public double getWeight() {
        return weight;
    }

    public double getPricePerKilo() {
        return pricePerKilo;
    }

    @Override
    public double deliveryPrice() {
        double deliveryPrice = weight * pricePerKilo;

        deliveryPrice += super.deliveryPrice();

        if (material.isFragile()) deliveryPrice += deliveryPrice * 0.01;

        return deliveryPrice;
    }
}

public class Main {
    public static void main(String[] args) throws IOException {
        BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(System.in));

        String materialName = bufferedReader.readLine();

        boolean isFragile = Integer.parseInt(bufferedReader.readLine().trim()) != 0;

        boolean toClientsAddress = Integer.parseInt(bufferedReader.readLine().trim()) != 0;

        double minPrice = Double.parseDouble(bufferedReader.readLine().trim());
        if (minPrice <= 0) minPrice = 1;

        double weight = Double.parseDouble(bufferedReader.readLine().trim());
        if (weight <= 0) weight = 1;

        double pricePerKg = Double.parseDouble(bufferedReader.readLine().trim());
        if (pricePerKg <= 0) pricePerKg = 1;

        double additionalPrice = Double.parseDouble(bufferedReader.readLine().trim());
        if (additionalPrice <= 0) additionalPrice = 1;

        Material material = new Material(materialName, isFragile);

        // Create a reference of type Deliverable and instantiate it with a Document.
        Deliverable deliverable = new Document(toClientsAddress, minPrice);
        // If the shipment is delivered to the client's address, set the additional price.
        if (toClientsAddress) {
            ((Document) deliverable).setAdditionalPrice(additionalPrice);
        }
        // Print the total shipping price for the Document.
        System.out.println(deliverable.deliveryPrice());

        // Now create a Deliverable reference for a WeightedItem.
        deliverable = new WeightedItem(toClientsAddress, material, weight, pricePerKg);
        // Set the additional price if delivery is required to the client's address.
        if (toClientsAddress) {
            ((WeightedItem) deliverable).setAdditionalPrice(additionalPrice);
        }
        // Print the total shipping price for the WeightedItem.
        System.out.println(deliverable.deliveryPrice());

        bufferedReader.close();
    }
}