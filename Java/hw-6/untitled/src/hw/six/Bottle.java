package hw.six;

public class Bottle {
    private final long id;
    private static long numberOfInstances = 0;
    private MaterialType materialType;

    public Bottle(MaterialType materialType) {
        numberOfInstances++;
        this.id = numberOfInstances;
        this.materialType = materialType;
    }

    public long getId() {
        return id;
    }

    public MaterialType getMaterialType() {
        return materialType;
    }

    @Override
    public String toString() {
        return "Bottle{" +
                "id=" + id +
                ", materialType=" + materialType +
                '}';
    }
}
