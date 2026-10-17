package com.mxerp.system.application.post.query;

import com.mxerp.shared.application.cqrs.Query;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.Setter;
import org.jspecify.annotations.NullMarked;

/** 查询所有岗位列表 */
@NullMarked
@Getter
@Setter
@Schema(description = "查询所有岗位列表")
public class GetPostsQuery implements Query {}
