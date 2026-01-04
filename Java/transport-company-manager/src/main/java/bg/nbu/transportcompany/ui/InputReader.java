package bg.nbu.transportcompany.ui;

import bg.nbu.transportcompany.exception.InvalidDataException;

import java.math.BigDecimal;
import java.time.LocalDate;
import java.util.Scanner;

public class InputReader {

    private final Scanner scanner = new Scanner(System.in);

    public int readInt(String prompt) {
        while (true) {
            System.out.print(prompt);
            String s = scanner.nextLine().trim();
            try {
                return Integer.parseInt(s);
            } catch (NumberFormatException ex) {
                System.out.println("Invalid number. Try again.");
            }
        }
    }

    public long readLong(String prompt) {
        while (true) {
            System.out.print(prompt);
            String s = scanner.nextLine().trim();
            try {
                return Long.parseLong(s);
            } catch (NumberFormatException ex) {
                System.out.println("Invalid number. Try again.");
            }
        }
    }

    public String readString(String prompt) {
        System.out.print(prompt);
        return scanner.nextLine().trim();
    }

    public BigDecimal readBigDecimal(String prompt) {
        while (true) {
            System.out.print(prompt);
            String s = scanner.nextLine().trim();
            try {
                return new BigDecimal(s);
            } catch (NumberFormatException ex) {
                System.out.println("Invalid decimal number. Try again.");
            }
        }
    }

    public LocalDate readDate(String prompt) {
        while (true) {
            System.out.print(prompt + " (YYYY-MM-DD): ");
            String s = scanner.nextLine().trim();
            try {
                return LocalDate.parse(s);
            } catch (Exception ex) {
                System.out.println("Invalid date. Try again.");
            }
        }
    }

    public <T extends Enum<T>> T readEnum(String prompt, Class<T> enumType) {
        while (true) {
            System.out.print(prompt + " " + enumOptions(enumType) + ": ");
            String s = scanner.nextLine().trim().toUpperCase();
            try {
                return Enum.valueOf(enumType, s);
            } catch (IllegalArgumentException ex) {
                System.out.println("Invalid value. Try again.");
            }
        }
    }

    public Integer readIntNullable(String prompt) {
        System.out.print(prompt + " (empty for null): ");
        String s = scanner.nextLine().trim();
        if (s.isEmpty()) {
            return null;
        }
        try {
            return Integer.parseInt(s);
        } catch (NumberFormatException ex) {
            throw new InvalidDataException("Invalid integer: " + s);
        }
    }

    public Double readDoubleNullable(String prompt) {
        System.out.print(prompt + " (empty for null): ");
        String s = scanner.nextLine().trim();
        if (s.isEmpty()) {
            return null;
        }
        try {
            return Double.parseDouble(s);
        } catch (NumberFormatException ex) {
            throw new InvalidDataException("Invalid number: " + s);
        }
    }

    private static <T extends Enum<T>> String enumOptions(Class<T> enumType) {
        StringBuilder sb = new StringBuilder("[");
        T[] values = enumType.getEnumConstants();
        for (int i = 0; i < values.length; i++) {
            if (i > 0) sb.append(", ");
            sb.append(values[i].name());
        }
        sb.append("]");
        return sb.toString();
    }
}
