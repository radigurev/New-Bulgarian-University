package hw.six;

import java.io.*;

public class SerializingService {
    public void serializeBottle(Bottle bottle, String filePath)
            throws IOException {

        try (ObjectOutputStream oos =
                     new ObjectOutputStream(new FileOutputStream(filePath))) {
            oos.writeObject(bottle);
        }
    }

    public Bottle deserializeBottle(String filePath)
            throws IOException, ClassNotFoundException {

        try (ObjectInputStream ois =
                     new ObjectInputStream(new FileInputStream(filePath))) {
            return (Bottle) ois.readObject();
        }
    }
}
