using System.Collections.Concurrent;
using GitlabSlackNotifier.Core.Domain.Gitlab.Projects;
using GitlabSlackNotifier.Core.Services.Gitlab;
using Microsoft.Extensions.Logging;

namespace GitlabSlackNotifier.Core.Infrastructures.Gitlab;

public class GitlabProjectsCache : IGitlabProjectsCache
{
    private readonly IGitlabProjectsClient _projectsClient;
    private readonly ILogger<IGitlabProjectsCache> _logger;
    private ConcurrentDictionary<string, GitlabProjectResponse> _projects = new(); // TODO: move to inline memory
    private ConcurrentDictionary<string, List<GitlabApprovalRulesResponse>> _mergeRequestRules = new(); // TODO: move to inline memory (!! important, so it can be cached for specific time)

    public GitlabProjectsCache (
        ILogger<IGitlabProjectsCache> logger,
        IGitlabProjectsClient projectsClient)
    {
        _projectsClient = projectsClient;
        _logger = logger;
    }


    public async Task<GitlabProjectResponse?> GetProjectByNamespace(string namespacePath)
    {
        if (_projects.TryGetValue(namespacePath, out var project))
            return project;

        var projects = await _projectsClient.GetProjects(new ()
        {
            Search = namespacePath
        });

        var firstProject = projects.FirstOrDefault(x => x.PathWithNamespace.Equals(namespacePath));
        if (firstProject == null || !firstProject.PathWithNamespace.Equals(namespacePath))
            return null;

        _logger.LogInformation($"Loading project from API {namespacePath}, projectId={firstProject.Id}");
        _projects.TryAdd(firstProject.PathWithNamespace, firstProject);
        return firstProject;
    }

    public async Task<List<GitlabApprovalRulesResponse>> GetRulesForMergeRequest(long projectId, int mergeRequestId)
    {
        var key = $"{projectId}-{mergeRequestId}";
        if (_mergeRequestRules.TryGetValue(key, out var rule))
            return rule;

        var rules = await _projectsClient.GetApprovalRules(projectId, mergeRequestId);
        if (rules == null)
            return null;
        
        _logger.LogInformation($"Cached approval rules for project={projectId}, mr={mergeRequestId}");
        _mergeRequestRules.TryAdd(key, rules);

        return rules;
    }
}
