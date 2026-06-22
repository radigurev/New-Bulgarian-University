package com.inf.cscb869_olympics.util;

import lombok.RequiredArgsConstructor;
import org.modelmapper.ModelMapper;
import org.springframework.stereotype.Component;

import java.util.List;

@Component
@RequiredArgsConstructor
public class MapperUtil {

    private final ModelMapper modelMapper;

    public <S, T> T map(S source, Class<T> targetType) {
        return modelMapper.map(source, targetType);
    }

    public <S, T> List<T> mapList(List<S> source, Class<T> targetType) {
        return source.stream().map(s -> modelMapper.map(s, targetType)).toList();
    }
}
