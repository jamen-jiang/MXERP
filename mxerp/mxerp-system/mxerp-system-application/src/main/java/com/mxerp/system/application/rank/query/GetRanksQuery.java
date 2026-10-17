package com.mxerp.system.application.rank.query;

import com.mxerp.shared.application.cqrs.Query;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.Setter;
import org.jspecify.annotations.NullMarked;

/** 查询所有职级列表 */
@NullMarked
@Getter
@Setter
@Schema(description = "查询所有职级列表")
public class GetRanksQuery implements Query {}
