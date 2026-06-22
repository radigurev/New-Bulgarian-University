package com.inf.cscb869_olympics.service;

import com.inf.cscb869_olympics.data.entity.User;
import com.inf.cscb869_olympics.dto.RegisterUserDTO;
import org.springframework.security.core.userdetails.UserDetailsService;

public interface UserService extends UserDetailsService {

    User registerAthlete(RegisterUserDTO dto);

    User findByUsername(String username);
}
