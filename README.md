# 116-backend
116 is a bold digital platform that promotes music and hip-hop culture in DR and beyond. Through articles, video shows, and exclusive behind-the-scenes content, it connects fans with artists, highlights emerging talent, and tells the stories shaping the culture.

## Git Workflow

This project follows a structured branching strategy designed for scalable development and reliable releases.

### Branch Structure

#### Main Branch (`main`)
The `main` branch contains the most up-to-date, production-ready code. All release branches are created from this branch, and it serves as the single source of truth for the application's current state.

#### Release Branches (`x.y.z`)
Release branches follow [Semantic Versioning (SemVer)](https://semver.org/) specification:
- **Format**: `MAJOR.MINOR.PATCH` (e.g., `1.0.0`, `1.2.3`, `2.0.0`)
- **MAJOR**: Breaking changes that are not backward compatible
- **MINOR**: New features that are backward compatible
- **PATCH**: Bug fixes that are backward compatible

Release branches are created directly from `main` and represent stable versions of the application.

#### Development Branch (`develop`)
The `develop` branch is the integration branch where all feature development converges. It contains the latest development changes and serves as the staging area before merging into `main`.

**Important**: The `develop` branch must always be rebased from the latest version branch to maintain a clean history and incorporate the most recent stable changes.

#### Feature Branches
All development work is done in feature branches that follow a strict naming convention:

**Pattern**: `^(feat|chore|bug|fix|doc|docs|style|refactor|perf|test|build|ci|revert|)-[a-z]+(-[a-z]+)*$`

**Branch Types**:
- `feat-`: New features or functionality
- `chore-`: Maintenance tasks, dependency updates
- `bug-`: Bug fixes and issue resolution
- `fix-`: General fixes and corrections
- `doc-`/`docs-`: Documentation updates
- `style-`: Code style, formatting changes
- `refactor-`: Code refactoring without functionality changes
- `perf-`: Performance improvements
- `test-`: Test additions or modifications
- `build-`: Build system or deployment changes
- `ci-`: Continuous integration configuration
- `revert-`: Reverting previous changes

**Examples**:
- `feat-user-authentication`
- `fix-api-response-validation`
- `chore-update-dependencies`
- `docs-git-workflow`

### Workflow Rules

1. **Feature Branch Creation**: All feature branches must be created from and regularly rebased against the `develop` branch
2. **Development Integration**: Feature branches are merged into `develop` via pull requests
3. **Release Preparation**: When `develop` is stable, it's merged into the latest release version, then merged into `main` for production release.
4. **Version Management**: Release branches are tagged and created from `main`
5. **Hotfixes**: Critical fixes can be branched directly from release branches and merged back to both `main` and `develop`

### Best Practices

- Keep feature branches focused on single features or fixes
- Rebase regularly to maintain clean commit history
- Use descriptive commit messages following conventional commit format
- Ensure all tests pass before merging
- Review code through pull requests before integration


