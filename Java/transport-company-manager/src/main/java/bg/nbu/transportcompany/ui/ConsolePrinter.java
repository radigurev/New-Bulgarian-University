package bg.nbu.transportcompany.ui;

import java.util.List;
import java.util.function.Function;

public final class ConsolePrinter {

    private ConsolePrinter() {
    }

    public static <T> void printList(String title, List<T> items, Function<T, String> formatter) {
        System.out.println();
        System.out.println("=== " + title + " ===");
        if (items == null || items.isEmpty()) {
            System.out.println("(no records)");
            return;
        }
        for (T item : items) {
            System.out.println(formatter.apply(item));
        }
    }
}
