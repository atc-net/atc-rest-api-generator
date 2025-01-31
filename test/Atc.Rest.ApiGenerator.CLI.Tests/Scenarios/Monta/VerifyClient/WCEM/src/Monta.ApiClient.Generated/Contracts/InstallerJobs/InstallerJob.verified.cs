﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.InstallerJobs;

/// <summary>
/// InstallerJob.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class InstallerJob
{
    /// <summary>
    /// Id of this installer job.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Id of the site related to this installer job.
    /// </summary>
    public long SiteId { get; set; }

    /// <summary>
    /// The operator of this installer job.
    /// </summary>
    [Required]
    public Operator Operator { get; set; }

    /// <summary>
    /// Id of the team of this installer job.
    /// </summary>
    public long TeamId { get; set; }

    /// <summary>
    /// Description of this installer job, will be visible to the installer.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Public link of this installer job, shareable with the installer.
    /// </summary>
    [Required]
    public string PublicLink { get; set; }

    /// <summary>
    /// Number of charge points included in this installer job.
    /// </summary>
    public int ChargePointCount { get; set; }

    /// <summary>
    /// Number of active (finished) charge points in this installer job.
    /// </summary>
    public int ChargePointActiveCount { get; set; }

    /// <summary>
    /// Email of the installer assigned to this job.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// List of tasks of this installer job.
    /// </summary>
    [Required]
    public List<InstallerJobTask> Tasks { get; set; }

    /// <summary>
    /// Date this installer job was completed.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>
    /// Date this installer job was created.
    /// </summary>
    [Required]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Date this installer job was updated.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Date this installer job was deleted.
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(SiteId)}: {SiteId}, {nameof(Operator)}: ({Operator}), {nameof(TeamId)}: {TeamId}, {nameof(Description)}: {Description}, {nameof(PublicLink)}: {PublicLink}, {nameof(ChargePointCount)}: {ChargePointCount}, {nameof(ChargePointActiveCount)}: {ChargePointActiveCount}, {nameof(Email)}: {Email}, {nameof(Tasks)}.Count: {Tasks?.Count ?? 0}, {nameof(CompletedAt)}: ({CompletedAt}), {nameof(CreatedAt)}: ({CreatedAt}), {nameof(UpdatedAt)}: ({UpdatedAt}), {nameof(DeletedAt)}: ({DeletedAt})";
}