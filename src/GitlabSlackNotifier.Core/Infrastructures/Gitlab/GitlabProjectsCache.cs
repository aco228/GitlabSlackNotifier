using System.Collections.Concurrent;
using GitlabSlackNotifier.Core.Domain.Gitlab.Projects;
using GitlabSlackNotifier.Core.Services.Gitlab;

namespace GitlabSlackNotifier.Core.Infrastructures.Gitlab;

public class GitlabProjectsCache : IGitlabProjectsCache
{
    private readonly IGitlabProjectsClient _projectsClient;
    private ConcurrentDictionary<string, GitlabProjectResponse> _projects = new();

    public GitlabProjectsCache (IGitlabProjectsClient projectsClient)
    {
        _projectsClient = projectsClient;
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

        _projects.TryAdd(firstProject.PathWithNamespace, firstProject);
        return firstProject;
    }
}