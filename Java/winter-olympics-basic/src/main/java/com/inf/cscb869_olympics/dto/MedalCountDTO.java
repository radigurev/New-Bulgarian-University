package com.inf.cscb869_olympics.dto;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import lombok.ToString;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@ToString
public class MedalCountDTO {

    private String country;
    private long gold;
    private long silver;
    private long bronze;

    public long getTotal() {
        return gold + silver + bronze;
    }
}
