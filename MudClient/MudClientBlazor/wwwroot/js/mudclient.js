(function () {
	let sendCommandHelper = null;

	window.registerSendCommandHandler = function (dotNetHelper) {
		sendCommandHelper = dotNetHelper;
	};

	window.disposeSendCommandHandler = function () {
		sendCommandHelper = null;
	};

	document.addEventListener('click', function (event) {
		const link = event.target.closest && event.target.closest('[data-mxp-command-id]');
		if (!link) {
			return;
		}

		event.preventDefault();
		const commandId = Number.parseInt(link.dataset.mxpCommandId, 10);
		if (sendCommandHelper && Number.isInteger(commandId)) {
			sendCommandHelper.invokeMethodAsync('SendCommandById', commandId);
		}
	});

	window.mudClientHotkeys = (function () {
		let dotNetHelper = null;
		let keydownHandler = null;
		let enabled = true;
		let boundCodes = new Set();

		function isNumpadCode(code) {
			return code && code.indexOf('Numpad') === 0;
		}

		function isPrintableKey(event) {
			return typeof event.key === 'string' && event.key.length === 1;
		}

		function isEditableTarget(target) {
			return !!(target && target.closest && target.closest('input, textarea, select, [contenteditable]'));
		}

		function shouldIgnoreEvent(event, code) {
			if (!enabled || event.defaultPrevented || event.ctrlKey || event.altKey || event.metaKey || event.shiftKey) {
				return true;
			}

			const target = event.target;
			if (isEditableTarget(target) && isPrintableKey(event) && !isNumpadCode(code)) {
				return true;
			}

			return !!(target && target.closest && target.closest('[data-hotkey-settings="true"]'));
		}

		function handleKeydown(event) {
			const code = event.code || '';
			if (!code || shouldIgnoreEvent(event, code) || !boundCodes.has(code)) {
				return;
			}

			event.preventDefault();
			event.stopPropagation();
			if (event.stopImmediatePropagation) {
				event.stopImmediatePropagation();
			}

			if (dotNetHelper) {
				dotNetHelper.invokeMethodAsync('HandleHotkeyCode', code);
			}
		}

		return {
			register: function (helper) {
				if (keydownHandler) {
					document.removeEventListener('keydown', keydownHandler, true);
				}

				dotNetHelper = helper;
				keydownHandler = handleKeydown;
				document.addEventListener('keydown', keydownHandler, true);
			},
			setBoundCodes: function (codes) {
				boundCodes = new Set(codes || []);
			},
			setEnabled: function (value) {
				enabled = !!value;
			},
			dispose: function () {
				if (keydownHandler) {
					document.removeEventListener('keydown', keydownHandler, true);
				}

				keydownHandler = null;
				dotNetHelper = null;
				boundCodes = new Set();
			}
		};
	})();

	window.mudClientInput = (function () {
		let element = null;
		let handler = null;
		let dotNetHelper = null;

		function isAtHistoryBoundary(direction) {
			if (!element || element.selectionStart !== element.selectionEnd) {
				return false;
			}

			const input = element.value || '';
			if (direction === 'up') {
				return input.lastIndexOf('\n', element.selectionStart - 1) === -1;
			}

			return input.indexOf('\n', element.selectionEnd) === -1;
		}

		function requestHistoryNavigation(event, direction) {
			if (!isAtHistoryBoundary(direction) || !dotNetHelper) {
				return;
			}

			event.preventDefault();
			const jumpToBoundary = event.ctrlKey;
			dotNetHelper.invokeMethodAsync('NavigateCommandHistory', direction, jumpToBoundary, element.value || '');
		}

		return {
			register: function (elementId, helper) {
				this.dispose();
				element = document.getElementById(elementId);
				if (!element) {
					return;
				}
				dotNetHelper = helper;

				handler = function (event) {
					if (event.key === 'Enter' && !event.shiftKey) {
						event.preventDefault();
						return;
					}

					if (!event.shiftKey && !event.altKey && !event.metaKey &&
						(event.key === 'ArrowUp' || event.key === 'ArrowDown')) {
						requestHistoryNavigation(event, event.key === 'ArrowUp' ? 'up' : 'down');
					}
				};
				element.addEventListener('keydown', handler);
			},
			dispose: function () {
				if (element && handler) {
					element.removeEventListener('keydown', handler);
				}

				element = null;
				handler = null;
				dotNetHelper = null;
			}
		};
	})();

	window.mudClientTranscript = (function () {
		let element = null;
		let mutationObserver = null;
		let scrollHandler = null;
		let keydownHandler = null;
		let interactionHandler = null;
		let contentLoadHandler = null;
		let isPinnedToBottom = true;
		let scrollFrame = null;
		let observedScrollHeight = 0;
		let hasUserScrollIntent = false;

		function isAtBottom() {
			return !!element && element.scrollTop + element.clientHeight >= element.scrollHeight - 4;
		}

		function updatePinnedState() {
			if (!element) {
				return;
			}

			const currentScrollHeight = element.scrollHeight;
			if (!hasUserScrollIntent && isPinnedToBottom && currentScrollHeight !== observedScrollHeight) {
				scheduleScrollToBottom();
				return;
			}

			isPinnedToBottom = isAtBottom();
			observedScrollHeight = currentScrollHeight;
			hasUserScrollIntent = false;
		}

		function noteUserScrollIntent() {
			hasUserScrollIntent = true;
			if (scrollFrame !== null) {
				window.cancelAnimationFrame(scrollFrame);
				scrollFrame = null;
			}
		}

		function scrollToBottom() {
			if (!element) {
				return;
			}

			element.scrollTop = element.scrollHeight;
			isPinnedToBottom = true;
			observedScrollHeight = element.scrollHeight;
			hasUserScrollIntent = false;
		}

		function scheduleScrollToBottom() {
			if (scrollFrame !== null || !isPinnedToBottom) {
				return;
			}

			scrollFrame = window.requestAnimationFrame(function () {
				scrollFrame = null;
				if (isPinnedToBottom) {
					scrollToBottom();
				}
			});
		}

		function scrollBy(amount) {
			if (!element) {
				return;
			}

			noteUserScrollIntent();
			element.scrollTop += amount;
			updatePinnedState();
		}

		function handleKeydown(event) {
			if (!element || event.altKey || event.metaKey) {
				return;
			}

			switch (event.key) {
				case 'Home':
					event.preventDefault();
					noteUserScrollIntent();
					element.scrollTop = 0;
					updatePinnedState();
					break;
				case 'End':
					event.preventDefault();
					scrollToBottom();
					break;
				case 'PageUp':
					event.preventDefault();
					scrollBy(-element.clientHeight);
					break;
				case 'PageDown':
					event.preventDefault();
					scrollBy(element.clientHeight);
					break;
				case 'ArrowUp':
					event.preventDefault();
					scrollBy(-Math.max(24, parseFloat(getComputedStyle(element).lineHeight) || 24));
					break;
				case 'ArrowDown':
					event.preventDefault();
					scrollBy(Math.max(24, parseFloat(getComputedStyle(element).lineHeight) || 24));
					break;
			}
		}

		return {
			register: function (outputElement) {
				this.dispose();
				element = outputElement;
				if (!element) {
					return;
				}

				updatePinnedState();
				scrollToBottom();
				scrollHandler = updatePinnedState;
				keydownHandler = handleKeydown;
				interactionHandler = noteUserScrollIntent;
				contentLoadHandler = scheduleScrollToBottom;
				element.addEventListener('scroll', scrollHandler, { passive: true });
				element.addEventListener('keydown', keydownHandler);
				element.addEventListener('wheel', interactionHandler, { passive: true });
				element.addEventListener('touchstart', interactionHandler, { passive: true });
				element.addEventListener('pointerdown', interactionHandler, { passive: true });
				element.addEventListener('load', contentLoadHandler, true);
				mutationObserver = new MutationObserver(scheduleScrollToBottom);
				mutationObserver.observe(element, { childList: true, subtree: true });
			},
			dispose: function () {
				if (element && scrollHandler) {
					element.removeEventListener('scroll', scrollHandler);
				}

				if (element && keydownHandler) {
					element.removeEventListener('keydown', keydownHandler);
				}

				if (element && interactionHandler) {
					element.removeEventListener('wheel', interactionHandler);
					element.removeEventListener('touchstart', interactionHandler);
					element.removeEventListener('pointerdown', interactionHandler);
				}

				if (element && contentLoadHandler) {
					element.removeEventListener('load', contentLoadHandler, true);
				}

				if (mutationObserver) {
					mutationObserver.disconnect();
				}

				if (scrollFrame !== null) {
					window.cancelAnimationFrame(scrollFrame);
				}

				element = null;
				mutationObserver = null;
				scrollHandler = null;
				keydownHandler = null;
				interactionHandler = null;
				contentLoadHandler = null;
				isPinnedToBottom = true;
				scrollFrame = null;
				observedScrollHeight = 0;
				hasUserScrollIntent = false;
			}
		};
	})();

	window.getSelectionStart = function (elementId) {
		const element = document.getElementById(elementId);
		return element ? element.selectionStart : 0;
	};

	window.getSelectionEnd = function (elementId) {
		const element = document.getElementById(elementId);
		return element ? element.selectionEnd : 0;
	};

	window.setSelectionRange = function (elementId, startPosition, endPosition) {
		const element = document.getElementById(elementId);
		if (!element) {
			return;
		}

		element.focus();
		const end = typeof endPosition === 'number' ? endPosition : startPosition;
		element.setSelectionRange(startPosition, end);
	};

	window.triggerDownload = function (filename, dataUri) {
		const link = document.createElement('a');
		link.href = dataUri;
		link.download = filename;
		link.click();
	};
})();
