package com.mxerp.system.domain.company;

import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 公司联系人数据
 *
 * @param name 姓名
 * @param telephone 电话（可空）
 * @param area 地区（可空）
 * @param address 地址（可空）
 */
@NullMarked
public record CompanyContactData(
    String name, @Nullable String telephone, @Nullable String area, @Nullable String address) {}
