package org.example;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;

class Manufacturer {
    String name;
    boolean providesExtendedWarranty;

    public Manufacturer(String name, boolean providesExtendedWarranty) {
        setName(name);
        setProvidesExtendedWarranty(providesExtendedWarranty);
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public boolean isProvidesExtendedWarranty() {
        return providesExtendedWarranty;
    }

    public void setProvidesExtendedWarranty(boolean providesExtendedWarranty) {
        this.providesExtendedWarranty = providesExtendedWarranty;
    }
}

class ElectricDevice {
    Manufacturer manufacturer;
    int minWarrantyMonths;

    public ElectricDevice(Manufacturer manufacturer, int minWarrantyMonths) {
        setManufactuer(manufacturer);
        setMinWarrantyMonths(minWarrantyMonths);
    }

    public Manufacturer getManufacturer() {
        return this.manufacturer;
    }

    public void setManufactuer(Manufacturer manufacturer) {
        this.manufacturer = manufacturer;
    }

    public int getMinWarrantyMonths() {
        return this.minWarrantyMonths;
    }

    public void setMinWarrantyMonths(int minWarrantyMonths) {
        if(minWarrantyMonths < 6) minWarrantyMonths = 6;

        this.minWarrantyMonths = minWarrantyMonths;
    }

    public int warranty() {
        return this.minWarrantyMonths + (this.manufacturer.isProvidesExtendedWarranty() ? 12 : 0);
    }
}

class Cooker extends ElectricDevice {
    boolean isGas;

    public Cooker(Manufacturer manufacturer, int minWarrantyMonths, boolean isGas) {
        super(manufacturer, minWarrantyMonths);
        setGas(isGas);
    }

    public boolean isGas() {
        return isGas;
    }

    public void setGas(boolean gas) {
        isGas = gas;
    }

    @Override
    public int warranty() {
        return super.warranty() + (isGas() ? 12 : 0);
    }
}

class WashingMachine extends ElectricDevice {
    boolean hasDryingOption;

    public WashingMachine(Manufacturer manufacturer, int minWarrantyMonths, boolean hasDryingOption) {
        super(manufacturer, minWarrantyMonths);

        setHasDryingOption(hasDryingOption);
    }

    public boolean isHasDryingOption() {
        return hasDryingOption;
    }

    public void setHasDryingOption(boolean hasDryingOption) {
        this.hasDryingOption = hasDryingOption;
    }

    @Override
    public  int warranty() {
        return  super.warranty() + (isHasDryingOption() ? minWarrantyMonths / 2 : 0);
    }
}

public class Solution {
    public static void main(String[] args) throws IOException {
        BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(System.in));

        String manufacturerName = bufferedReader.readLine();

        boolean isLongTermWarranty = Integer.parseInt(bufferedReader.readLine().trim()) != 0;

        int minWarranty = Integer.parseInt(bufferedReader.readLine().trim());

        boolean isGas = Integer.parseInt(bufferedReader.readLine().trim()) != 0;

        boolean isDryer = Integer.parseInt(bufferedReader.readLine().trim()) != 0;

        // Create object of type Manufacturer using the constructor with two parameters. Pass manufacturerName and isLongTermWarranty as arguments
        Manufacturer manufacturer = new Manufacturer(manufacturerName, isLongTermWarranty);

// Create object of type ElectricDevice using the constructor with 2 parameters. Pass manufacturer and minWarranty as arguments: ElectricDevice electricDevice = new ElectricDevice(manufacturer, minWarranty);
        ElectricDevice electricDevice = new ElectricDevice(manufacturer, minWarranty);

// Print on the console the warranty of the the object electricDevice, by calling warranty() method
        System.out.println(electricDevice.warranty());

// Assign the electricDevice a new object of type Cooker using the constructor with 3 parameters. Pass manufacturer, minWarranty and isGas as arguments: electricDevice = new Cooker(manufacturer, minWarranty, isGas);
        electricDevice = new Cooker(manufacturer, minWarranty, isGas);

// Print on the console the warranty of the object electricDevice, by calling warranty() method
        System.out.println(electricDevice.warranty());

// Assign the electricDevice a new object of type WashingMachine using the constructor with 3 parameters. Pass manufacturer, minWarranty and isDryer as arguments: electricDevice = new WashingMachine(manufacturer, minWarranty, isDryer);
        electricDevice = new WashingMachine(manufacturer, minWarranty, isDryer);

// Print on the console the warranty of the the object electricDevice, by calling warranty() method
        System.out.println(electricDevice.warranty());

        bufferedReader.close();
    }
}