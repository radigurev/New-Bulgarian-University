package org.example;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.stream.Collectors;

public class Main {
    public static void main(String[] args) {
        ArrayList<String> list = new ArrayList<String>(Arrays.asList("Buenos Aires", "Córdoba", "La Plata", "Sofia", "Burgas", "Plovdiv"));

        for (int i = 0; i < list.size(); i++) {
            if(!list.get(i).contains(" ")) continue;

            System.out.println(list.get(i));
        }

        list.stream().filter(x -> x.contains(" ")).forEach(System.out::println);

        ArrayList<String> list2 = new ArrayList<String>(Arrays.asList("1", "2", "3", "4", "5", "6"));
        ArrayList<Integer> list3 = new ArrayList<Integer>();

        for (int i = 0; i < list2.size(); i++) {
            list3.add(Integer.parseInt(list2.get(i)));
        }

        ArrayList<String> strings = list2.stream().map(x -> x+x).collect(Collectors.toCollection(ArrayList::new));
        strings.forEach(System.out::println);

        ArrayList<Integer> intList = list2.stream().map(x -> Integer.parseInt(x)).collect(Collectors.toCollection(ArrayList::new));
        intList.forEach(System.out::println);
    }
}