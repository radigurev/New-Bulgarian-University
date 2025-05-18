import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.*;
import java.util.stream.Collectors;
import java.util.stream.Stream;

import static java.util.stream.Collectors.toList;
class Vehicle {
    private long id;
    private String brand;
    private boolean emergencyVehicle;
    private double weight;
    private VehicleType vehicleType;
    public Vehicle(long id, String brand, boolean emergencyVehicle, double weight, VehicleType vehicleType) {
        this.id = id;
        this.brand = brand;
        this.emergencyVehicle = emergencyVehicle;
        this.weight = weight;
        this.vehicleType = vehicleType;
    }
    public long getId() {
        return id;
    }
    public String getBrand() {
        return brand;
    }
    public boolean isEmergencyVehicle() {
        return emergencyVehicle;
    }
    public double getWeight() {
        return weight;
    }

    public VehicleType getVehicleType() {
        return vehicleType;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        Vehicle vehicle = (Vehicle) o;
        return id == vehicle.id;
    }

    @Override
    public int hashCode() {
        return Objects.hash(id);
    }

    @Override
    public String toString() {
        return "Vehicle{" +
                "id=" + id +
                ", brand='" + brand + '\'' +
                ", emergencyVehicle=" + emergencyVehicle +
                ", weight=" + weight +
                ", vehicleType=" + vehicleType +
                '}';
    }
}

enum VehicleType {
    CAR, TRUCK, BUS
}

class VehicleServiceImpl {
    public Comparator<Vehicle> comparatorByWeight() {
        return Comparator.comparingDouble(Vehicle::getWeight);
    }

    public Comparator<Vehicle> comparatorByEmergency() {
        return Comparator.comparing(Vehicle::isEmergencyVehicle);
    }

    public Comparator<Vehicle> comparatorById() {
        return Comparator.comparingLong(Vehicle::getId);
    }

    public Set<Vehicle> addVehiclesToSet(List<Vehicle> vehicles) {
        return vehicles.stream()
                .collect(Collectors.toCollection(LinkedHashSet::new));
    }

    public void printVehiclesInTreeSet(Set<Vehicle> vehicles) {
        Comparator<Vehicle> treeComparator =
                comparatorByEmergency()
                        .thenComparing(comparatorById());

        TreeSet<Vehicle> treeSet = new TreeSet<>(treeComparator);
        treeSet.addAll(vehicles);

        treeSet.forEach(System.out::println);
    }

    public double sumVehiclesWeight(Set<Vehicle> vehicles) {
        return vehicles.stream()
                .mapToDouble(Vehicle::getWeight)
                .sum();
    }

    public long countVehiclesByType(Set<Vehicle> vehicles, VehicleType vehicleType) {
        return vehicles.stream()
                .filter(v -> v.getVehicleType() == vehicleType)
                .count();
    }

    public Double minWeight(Set<Vehicle> vehicles) {
        return vehicles.stream()
                .mapToDouble(Vehicle::getWeight)
                .min()
                .orElse(0);
    }

    public Double averageWeight(Set<Vehicle> vehicles) {
        return vehicles.stream()
                .mapToDouble(Vehicle::getWeight)
                .average()
                .orElse(0);
    }
}

public class Solution {
    public static void main(String[] args) throws IOException {
        BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(System.in));

        List<Integer> idList = Stream.of(bufferedReader.readLine().replaceAll("\\s+$", "").split(" "))
                .map(Integer::parseInt)
                .collect(toList());

        List<String> brandList = Stream.of(bufferedReader.readLine().replaceAll("\\s+$", "").split(" "))
                .collect(toList());

        List<Boolean> emergencyList = Stream.of(bufferedReader.readLine().replaceAll("\\s+$", "").split(" "))
                .map(e -> Integer.parseInt(e) != 0)
                .collect(toList());

        List<Double> weightList = Stream.of(bufferedReader.readLine().replaceAll("\\s+$", "").split(" "))
                .map(Double::parseDouble)
                .collect(toList());

        List<String> vehicleTypeList = Stream.of(bufferedReader.readLine().replaceAll("\\s+$", "").split(" "))
                .collect(toList());

        String vehicleTypeString = bufferedReader.readLine();

        Vehicle vehicle1 = new Vehicle(idList.get(0).longValue(), brandList.get(0), emergencyList.get(0), weightList.get(0), VehicleType.valueOf(vehicleTypeList.get(0)));
        Vehicle vehicle2 = new Vehicle(idList.get(1).longValue(), brandList.get(1), emergencyList.get(1), weightList.get(1), VehicleType.valueOf(vehicleTypeList.get(1)));
        Vehicle vehicle3 = new Vehicle(idList.get(2).longValue(), brandList.get(2), emergencyList.get(2), weightList.get(2), VehicleType.valueOf(vehicleTypeList.get(2)));
        Vehicle vehicle4 = new Vehicle(idList.get(3).longValue(), brandList.get(3), emergencyList.get(3), weightList.get(3), VehicleType.valueOf(vehicleTypeList.get(3)));
        Vehicle vehicle5 = new Vehicle(idList.get(4).longValue(), brandList.get(4), emergencyList.get(4), weightList.get(4), VehicleType.valueOf(vehicleTypeList.get(4)));
        Vehicle vehicle6 = new Vehicle(idList.get(5).longValue(), brandList.get(5), emergencyList.get(5), weightList.get(5), VehicleType.valueOf(vehicleTypeList.get(5)));

        List<Vehicle> vehicles = Arrays.asList(vehicle1, vehicle2, vehicle3, vehicle4, vehicle5, vehicle6);

        VehicleServiceImpl vehicleService = new VehicleServiceImpl();

        Set<Vehicle> vehicleSet = vehicleService.addVehiclesToSet(vehicles);

        vehicleService.printVehiclesInTreeSet(vehicleSet);

        System.out.println();

        System.out.println(vehicleService.sumVehiclesWeight(vehicleSet));

        System.out.println(vehicleService.countVehiclesByType(vehicleSet, VehicleType.valueOf(vehicleTypeString)));

        System.out.println(vehicleService.minWeight(vehicleSet));

        System.out.println(vehicleService.averageWeight(vehicleSet));

        bufferedReader.close();
    }
}