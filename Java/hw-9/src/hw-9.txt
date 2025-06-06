import java.io.*;
import java.util.*;
import java.util.stream.*;
import static java.util.stream.Collectors.toList;

enum VehicleType {
    CAR, TRUCK, BUS
}

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

    public long getId() { return id; }
    public String getBrand() { return brand; }
    public boolean isEmergencyVehicle() { return emergencyVehicle; }
    public double getWeight() { return weight; }
    public VehicleType getVehicleType() { return vehicleType; }

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

    public Map<Vehicle, Double> putVehiclesPricesInHashMap(List<Vehicle> vehicles, List<Double> prices) {
        return IntStream.range(0, vehicles.size())
                .boxed()
                .collect(Collectors.toMap(
                        vehicles::get,
                        prices::get,
                        (oldVal, newVal) -> newVal,
                        LinkedHashMap::new
                ));
    }

    public void printVehiclesOrderedByVehicleType(Map<Vehicle, Double> vehiclesPrices) {
        vehiclesPrices.entrySet().stream()
                .sorted(Comparator.comparing(e -> e.getKey().getVehicleType()))
                .forEach(entry -> System.out.println(entry.getKey() + "=" + entry.getValue()));
    }

    public void printVehiclesOrderedByWeightAndPrice(Map<Vehicle, Double> vehiclesPrices) {
        vehiclesPrices.entrySet().stream()
                .sorted(Comparator
                        .comparing((Map.Entry<Vehicle, Double> e) -> e.getKey().getWeight())
                        .thenComparing(Map.Entry::getValue)
                )
                .forEach(entry -> System.out.println(entry.getKey() + "=" + entry.getValue()));
    }

    public long numberOfVehiclesFilteredByVehicleType(Map<Vehicle, Double> vehiclesPrices, VehicleType vehicleType) {
        return vehiclesPrices.keySet().stream()
                .filter(v -> v.getVehicleType() == vehicleType)
                .count();
    }

    public Double minVehiclesPrice(Map<Vehicle, Double> vehiclesPrices) {
        return vehiclesPrices.values().stream()
                .min(Double::compareTo)
                .orElse(null);
    }

    public Double averageVehiclesPriceByEmergency(Map<Vehicle, Double> vehiclesPrices) {
        return vehiclesPrices.entrySet().stream()
                .filter(e -> e.getKey().isEmergencyVehicle())
                .mapToDouble(Map.Entry::getValue)
                .average()
                .orElse(0.0);
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

        List<Double> priceList = Stream.of(bufferedReader.readLine().replaceAll("\\s+$", "").split(" "))
                .map(Double::parseDouble)
                .collect(toList());

        String vehicleTypeString = bufferedReader.readLine();

        List<Vehicle> vehicles = IntStream.range(0, idList.size())
                .mapToObj(i -> new Vehicle(
                        idList.get(i),
                        brandList.get(i),
                        emergencyList.get(i),
                        weightList.get(i),
                        VehicleType.valueOf(vehicleTypeList.get(i))
                ))
                .collect(toList());

        VehicleServiceImpl vehicleService = new VehicleServiceImpl();
        Map<Vehicle, Double> vehiclesPrices = vehicleService.putVehiclesPricesInHashMap(vehicles, priceList);

        vehicleService.printVehiclesOrderedByVehicleType(vehiclesPrices);
        System.out.println();

        vehicleService.printVehiclesOrderedByWeightAndPrice(vehiclesPrices);
        System.out.println();

        System.out.println(vehicleService.numberOfVehiclesFilteredByVehicleType(vehiclesPrices, VehicleType.valueOf(vehicleTypeString)));
        System.out.println(vehicleService.minVehiclesPrice(vehiclesPrices));
        System.out.println(vehicleService.averageVehiclesPriceByEmergency(vehiclesPrices));

        bufferedReader.close();
    }
}
