using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.Users;

/// <summary>
/// 用户绑定的岗位(对应 Java User_Post)
/// </summary>
public class UserPost : DeletionAuditedEntity
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public long? UserId { get; private set; }
    /// <summary>
    /// 岗位Id
    /// </summary>
    public long PostId { get; private set; }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected UserPost()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="postId">岗位Id</param>
    public UserPost(long postId)
    {
        PostId = postId;
    }
}
